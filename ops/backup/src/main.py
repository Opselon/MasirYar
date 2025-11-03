# /ops/backup/src/main.py
import os
import subprocess
import datetime
import logging
import time

logging.basicConfig(level=logging.INFO, format='%(asctime)s - %(levelname)s - %(message)s')

def get_env_variable(name, default=None):
    """Gets an environment variable or returns a default."""
    value = os.environ.get(name)
    if value is None and default is None:
        logging.error(f"Environment variable {name} not set.")
        raise ValueError(f"Environment variable {name} not set.")
    return value or default

def backup_postgres():
    """Backs up the PostgreSQL database."""
    pg_host = get_env_variable("PG_HOST")
    pg_user = get_env_variable("PG_USER")
    pg_password = get_env_variable("PG_PASSWORD")
    backup_dir = get_env_variable("BACKUP_DIR")

    timestamp = datetime.datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
    backup_file = os.path.join(backup_dir, f"postgres_backup_{timestamp}.sql")

    os.environ['PGPASSWORD'] = pg_password

    command = [
        "pg_dump",
        "-h", pg_host,
        "-U", pg_user,
        "-d", "PersonalGrowthDb", # Assuming the database name is static
        "-f", backup_file
    ]

    try:
        logging.info("Starting PostgreSQL backup...")
        subprocess.run(command, check=True, capture_output=True, text=True)
        logging.info(f"PostgreSQL backup successful: {backup_file}")
    except subprocess.CalledProcessError as e:
        logging.error(f"PostgreSQL backup failed: {e.stderr}")

def backup_redis():
    """Backs up the Redis database."""
    redis_host = get_env_variable("REDIS_HOST")
    backup_dir = get_env_variable("BACKUP_DIR")

    timestamp = datetime.datetime.now().strftime("%Y-%m-%d_%H-%M-%S")
    backup_file = os.path.join(backup_dir, f"redis_backup_{timestamp}.rdb")

    command = [
        "redis-cli",
        "-h", redis_host,
        "SAVE"
    ]

    try:
        logging.info("Starting Redis backup...")
        subprocess.run(command, check=True, capture_output=True, text=True)
        # The SAVE command saves to a dump.rdb file in the Redis data directory.
        # We need to find and move it. This is a simplification.
        # A better approach would be to configure Redis to save to a specific path.
        logging.info(f"Redis SAVE command successful. Backup file is on the Redis server.")
    except subprocess.CalledProcessError as e:
        logging.error(f"Redis backup failed: {e.stderr}")

def main():
    """Main function to run the backup process."""
    logging.info("Backup service started.")

    while True:
        backup_postgres()
        backup_redis()

        # In a real scenario, this would be a cron job,
        # but for this example, we'll run it every 24 hours.
        logging.info("Sleeping for 24 hours...")
        time.sleep(24 * 60 * 60)

if __name__ == "__main__":
    main()
