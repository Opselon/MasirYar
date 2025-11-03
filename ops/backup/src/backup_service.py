# /ops/backup/src/backup_service.py
# -*- coding: utf-8 -*-

import os
import subprocess
import logging
import sys
import shutil
import json
from datetime import datetime, timezone
from pathlib import Path

import psycopg2
import redis
from dotenv import load_dotenv

# --- پیکربندی اولیه ---
# بارگذاری متغیرهای محیطی از فایل .env
# در محیط پروداکشن، این متغیرها باید توسط Docker یا سیستم عامل فراهم شوند.
load_dotenv()

# تنظیمات لاگ‌گیری
LOG_FORMAT = '%(asctime)s - %(levelname)s - %(message)s'
logging.basicConfig(level=logging.INFO, format=LOG_FORMAT, stream=sys.stdout)


# --- خواندن متغیرهای محیطی با مقادیر پیش‌فرض ---
PG_HOST = os.getenv('PG_HOST', 'localhost')
PG_PORT = os.getenv('PG_PORT', '5432')
PG_USER = os.getenv('PG_USER')
PG_PASSWORD = os.getenv('PG_PASSWORD')
PG_DATABASES = os.getenv('PG_DATABASES', '').split(',')
REDIS_HOST = os.getenv('REDIS_HOST', 'localhost')
REDIS_PORT = int(os.getenv('REDIS_PORT', '6379'))
FILE_PATHS = [Path(p.strip()) for p in os.getenv('FILE_PATHS', '').split(',') if p.strip()]
BACKUP_DIR = Path(os.getenv('BACKUP_DIR', '/backups'))
RETENTION_DAYS = int(os.getenv('RETENTION_DAYS', 7))
ADVISORY_LOCK_KEY = int(os.getenv('ADVISORY_LOCK_KEY', 123456789))

# --- توابع اصلی سرویس بکاپ ---

def _get_db_connection():
    """یک اتصال جدید به PostgreSQL ایجاد می‌کند."""
    try:
        conn = psycopg2.connect(
            host=PG_HOST,
            port=PG_PORT,
            user=PG_USER,
            password=PG_PASSWORD,
            dbname='postgres' # اتصال به دیتابیس پیش‌فرض برای اجرای کوئری‌های مدیریتی
        )
        return conn
    except psycopg2.OperationalError as e:
        logging.error(f"خطا در اتصال به PostgreSQL: {e}")
        return None

def obtain_lock(conn):
    """
    یک قفل مشورتی (advisory lock) در PostgreSQL برای جلوگیری از اجرای همزمان ایجاد می‌کند.
    این روش از ایجاد فایل lock امن‌تر است، به خصوص در محیط‌های توزیع‌شده.
    """
    if not conn:
        return False
    try:
        with conn.cursor() as cur:
            cur.execute(f"SELECT pg_try_advisory_lock({ADVISORY_LOCK_KEY})")
            locked = cur.fetchone()[0]
            if not locked:
                logging.warning("یک فرآیند بکاپ دیگر در حال اجراست. از این اجرا صرف نظر می‌شود.")
                return False
            logging.info("قفل با موفقیت ایجاد شد.")
            return True
    except Exception as e:
        logging.error(f"خطا در ایجاد قفل: {e}")
        return False

def release_lock(conn):
    """قفل مشورتی را آزاد می‌کند."""
    if not conn:
        return
    try:
        with conn.cursor() as cur:
            cur.execute(f"SELECT pg_advisory_unlock({ADVISORY_LOCK_KEY})")
        logging.info("قفل با موفقیت آزاد شد.")
    except Exception as e:
        logging.error(f"خطا در آزاد کردن قفل: {e}")

def list_databases(conn):
    """لیست تمام دیتابیس‌ها را (به جز دیتابیس‌های template) برمی‌گرداند."""
    if PG_DATABASES and PG_DATABASES[0]:
        return PG_DATABASES

    if not conn:
        return []
    try:
        with conn.cursor() as cur:
            cur.execute("SELECT datname FROM pg_database WHERE datistemplate = false AND datname != 'postgres';")
            databases = [row[0] for row in cur.fetchall()]
            return databases
    except Exception as e:
        logging.error(f"خطا در دریافت لیست دیتابیس‌ها: {e}")
        return []

def backup_postgres_db(db_name: str, target_dir: Path):
    """از یک دیتابیس PostgreSQL با استفاده از pg_dump بکاپ می‌گیرد."""
    logging.info(f"شروع بکاپ از دیتابیس: {db_name}")
    dump_file = target_dir / f"pg-{db_name}.dump"

    # اطمینان از اینکه رمز عبور در متغیر محیطی است
    env = os.environ.copy()
    env['PGPASSWORD'] = PG_PASSWORD

    command = [
        'pg_dump',
        '-h', PG_HOST,
        '-p', PG_PORT,
        '-U', PG_USER,
        '-d', db_name,
        '-F', 'c', # فرمت سفارشی فشرده
        '-b', # شامل اشیاء بزرگ (blobs)
        '-v', # حالت پرجزئیات
        '-f', str(dump_file)
    ]
    try:
        subprocess.run(command, check=True, capture_output=True, text=True, env=env)
        logging.info(f"بکاپ از دیتابیس {db_name} با موفقیت در {dump_file} ذخیره شد.")
        return dump_file
    except subprocess.CalledProcessError as e:
        logging.error(f"خطا در اجرای pg_dump برای دیتابیس {db_name}: {e.stderr}")
        return None

def backup_redis():
    """از Redis با استفاده از BGSAVE بکاپ می‌گیرد."""
    logging.info("شروع بکاپ از Redis...")
    try:
        r = redis.Redis(host=REDIS_HOST, port=REDIS_PORT)
        last_save_time_before = r.lastsave()
        r.bgsave()

        # منتظر می‌مانیم تا BGSAVE تمام شود
        for _ in range(30): # حداکثر 30 ثانیه صبر می‌کنیم
            if r.lastsave() > last_save_time_before:
                logging.info("BGSAVE در Redis با موفقیت انجام شد.")
                # در یک سناریوی واقعی، باید مسیر dump.rdb را از Redis بگیریم
                # اما معمولاً در /data/dump.rdb است. این بخش به پیکربندی Docker بستگی دارد.
                # در اینجا فرض می‌کنیم که این فایل در یک volume مشترک قابل دسترسی است.
                return Path("/data/dump.rdb") # مسیر فرضی
        logging.warning("BGSAVE در Redis در زمان مقرر تمام نشد.")
        return None
    except Exception as e:
        logging.error(f"خطا در بکاپ‌گیری از Redis: {e}")
        return None

def backup_files(target_dir: Path):
    """از مسیرهای مشخص‌شده با استفاده از tar بکاپ می‌گیرد."""
    backup_files_list = []
    for path in FILE_PATHS:
        if not path.exists():
            logging.warning(f"مسیر فایل برای بکاپ یافت نشد: {path}")
            continue

        logging.info(f"شروع بکاپ از مسیر: {path}")
        archive_name = f"files-{path.name}.tar.gz"
        archive_path = target_dir / archive_name

        command = [
            'tar',
            '-czf', str(archive_path),
            '-C', str(path.parent), # برای حفظ ساختار نسبی
            str(path.name)
        ]
        try:
            subprocess.run(command, check=True, capture_output=True, text=True)
            logging.info(f"بکاپ از {path} با موفقیت در {archive_path} ذخیره شد.")
            backup_files_list.append(archive_path)
        except subprocess.CalledProcessError as e:
            logging.error(f"خطا در فشرده‌سازی مسیر {path}: {e.stderr}")
    return backup_files_list

def generate_manifest(backup_dir: Path, backup_files: dict):
    """یک فایل manifest.json با اطلاعات بکاپ ایجاد می‌کند."""
    manifest = {
        'timestamp_utc': datetime.now(timezone.utc).isoformat(),
        'backup_directory': str(backup_dir),
        'files': []
    }
    for name, path in backup_files.items():
        if path and path.exists():
            file_stat = path.stat()
            manifest['files'].append({
                'name': name,
                'path': str(path.relative_to(backup_dir)),
                'size_bytes': file_stat.st_size,
            })

    manifest_path = backup_dir / "manifest.json"
    with open(manifest_path, 'w') as f:
        json.dump(manifest, f, indent=2)
    logging.info(f"فایل manifest در {manifest_path} ایجاد شد.")

def rotate_backups():
    """بکاپ‌های قدیمی‌تر از RETENTION_DAYS را حذف می‌کند."""
    if RETENTION_DAYS <= 0:
        return

    logging.info(f"حذف بکاپ‌های قدیمی‌تر از {RETENTION_DAYS} روز...")
    now = datetime.now(timezone.utc)

    try:
        backup_dirs = [d for d in BACKUP_DIR.iterdir() if d.is_dir()]
        backup_dirs.sort(key=lambda d: d.name) # مرتب‌سازی بر اساس نام (که با timestamp شروع می‌شود)

        to_delete = backup_dirs[:-RETENTION_DAYS] # همه به جز N تای آخر

        for d in to_delete:
            logging.info(f"حذف پوشه بکاپ قدیمی: {d}")
            shutil.rmtree(d)
    except Exception as e:
        logging.error(f"خطا در حذف بکاپ‌های قدیمی: {e}")


def run_once():
    """یک بار فرآیند کامل بکاپ را اجرا می‌کند."""
    start_time = datetime.now(timezone.utc)
    logging.info("--- شروع فرآیند بکاپ ---")

    conn = _get_db_connection()
    if not obtain_lock(conn):
        sys.exit(2)

    try:
        # ۱. ایجاد پوشه بکاپ برای این اجرا
        timestamp = start_time.strftime('%Y%m%dT%H%M%SZ')
        today_backup_dir = BACKUP_DIR / timestamp
        today_backup_dir.mkdir(parents=True, exist_ok=True)
        logging.info(f"پوشه بکاپ ایجاد شد: {today_backup_dir}")

        all_backup_files = {}

        # ۲. بکاپ از PostgreSQL
        databases = list_databases(conn)
        if not databases:
            logging.warning("هیچ دیتابیسی برای بکاپ یافت نشد.")
        else:
            for db in databases:
                dump_file = backup_postgres_db(db, today_backup_dir)
                if dump_file:
                    all_backup_files[f'postgres_{db}'] = dump_file

        # ۳. بکاپ از Redis (بخش شبیه‌سازی شده)
        redis_dump_path = backup_redis()
        if redis_dump_path and redis_dump_path.exists():
            # مسیر dump.rdb را به پوشه بکاپ کپی می‌کنیم
            final_redis_path = today_backup_dir / "redis-dump.rdb"
            shutil.copy(redis_dump_path, final_redis_path)
            all_backup_files['redis'] = final_redis_path

        # ۴. بکاپ از فایل‌ها
        archived_files = backup_files(today_backup_dir)
        for i, path in enumerate(archived_files):
            all_backup_files[f'file_{i}'] = path

        # ۵. ایجاد manifest
        generate_manifest(today_backup_dir, all_backup_files)

        # ۶. حذف بکاپ‌های قدیمی
        rotate_backups()

        logging.info("--- فرآیند بکاپ با موفقیت به پایان رسید ---")

    except Exception as e:
        logging.critical(f"یک خطای پیش‌بینی‌نشده در فرآیند بکاپ رخ داد: {e}", exc_info=True)
        sys.exit(1)
    finally:
        release_lock(conn)
        if conn:
            conn.close()

if __name__ == '__main__':
    if len(sys.argv) > 1 and sys.argv[1] == 'run-once':
        run_once()
    else:
        # در حالت daemon، یک orchestrator خارجی (مانند cron) باید این اسکریپت را با run-once فراخوانی کند.
        print("برای اجرای یکباره، از دستور 'python backup_service.py run-once' استفاده کنید.")
