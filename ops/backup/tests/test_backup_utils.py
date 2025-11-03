# /ops/backup/tests/test_backup_utils.py
import os
import json
from pathlib import Path
from datetime import datetime, timedelta, timezone
from unittest.mock import patch, MagicMock

# برای اینکه بتوانیم ماژول backup_service را import کنیم، باید مسیر src را به sys.path اضافه کنیم.
import sys
sys.path.insert(0, str(Path(__file__).parent.parent / 'src'))

import backup_service

# --- تست‌های مربوط به حذف بکاپ‌های قدیمی (Retention) ---

def test_rotate_backups_deletes_oldest(tmp_path):
    """
    تست می‌کند که آیا تابع rotate_backups به درستی قدیمی‌ترین پوشه‌ها را حذف می‌کند
    و فقط تعداد مشخص‌شده را باقی می‌گذارد.
    """
    # ساخت پوشه‌های بکاپ ساختگی
    base_dir = tmp_path
    for i in range(10):
        # ایجاد timestampهای متوالی در گذشته
        ts = (datetime.now(timezone.utc) - timedelta(days=i)).strftime('%Y%m%dT%H%M%SZ')
        (base_dir / ts).mkdir()

    # استفاده از patch برای جایگزینی متغیرهای گلوبال با مقادیر تستی
    with patch('backup_service.BACKUP_DIR', base_dir), \
         patch('backup_service.RETENTION_DAYS', 7):

        backup_service.rotate_backups()

    # بررسی نتیجه
    remaining_dirs = sorted([d.name for d in base_dir.iterdir()])
    assert len(remaining_dirs) == 7
    # قدیمی‌ترین پوشه حذف شده باید پوشه‌ای باشد که 9 روز پیش ایجاد شده است
    assert "T" in remaining_dirs[0] # اطمینان از فرمت صحیح نام
    assert (datetime.now(timezone.utc) - timedelta(days=9)).strftime('%Y%m%d') not in "".join(remaining_dirs)


def test_rotate_backups_does_nothing_if_not_enough_backups(tmp_path):
    """
    تست می‌کند که اگر تعداد بکاپ‌ها کمتر از retention_days باشد، هیچ پوشه‌ای حذف نمی‌شود.
    """
    base_dir = tmp_path
    for i in range(5):
        (base_dir / f"{i}").mkdir()

    with patch('backup_service.BACKUP_DIR', base_dir), \
         patch('backup_service.RETENTION_DAYS', 7):

        backup_service.rotate_backups()

    assert len(list(base_dir.iterdir())) == 5


# --- تست‌های مربوط به ایجاد Manifest ---

def test_generate_manifest_creates_correct_json(tmp_path):
    """
    تست می‌کند که آیا فایل manifest.json با محتوای صحیح ایجاد می‌شود.
    """
    backup_dir = tmp_path

    # ایجاد فایل‌های بکاپ ساختگی
    pg_dump_path = backup_dir / "pg-testdb.dump"
    pg_dump_path.write_text("postgres dump content")

    redis_dump_path = backup_dir / "redis-dump.rdb"
    redis_dump_path.write_text("redis dump content")

    backup_files = {
        'postgres_testdb': pg_dump_path,
        'redis': redis_dump_path
    }

    backup_service.generate_manifest(backup_dir, backup_files)

    # بررسی محتوای manifest.json
    manifest_path = backup_dir / "manifest.json"
    assert manifest_path.exists()

    with open(manifest_path, 'r') as f:
        manifest_data = json.load(f)

    assert "timestamp_utc" in manifest_data
    assert manifest_data['backup_directory'] == str(backup_dir)
    assert len(manifest_data['files']) == 2

    pg_file_info = next(f for f in manifest_data['files'] if f['name'] == 'postgres_testdb')
    assert pg_file_info['path'] == "pg-testdb.dump"
    assert pg_file_info['size_bytes'] == len("postgres dump content")

    redis_file_info = next(f for f in manifest_data['files'] if f['name'] == 'redis')
    assert redis_file_info['path'] == "redis-dump.rdb"
    assert redis_file_info['size_bytes'] == len("redis dump content")
