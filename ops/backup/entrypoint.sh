#!/bin/bash
# /ops/backup/entrypoint.sh

set -e

# --- Environment Variable Setup for Cron ---
# Export all environment variables to a file that cron can use
printenv | grep -v "no_proxy" > /etc/environment

# --- Cron Job Setup ---
echo "Setting up cron schedule: ${CRON_SCHEDULE}"

# Create a crontab file
# The command runs the backup service once and logs the output
echo "${CRON_SCHEDULE} /usr/local/bin/python /app/src/backup_service.py run-once >> /var/log/cron.log 2>&1" > /etc/cron.d/backup-cron

# Give execution rights on the cron job
chmod 0644 /etc/cron.d/backup-cron

# Apply cron job
crontab /etc/cron.d/backup-cron

# Create the log file to be able to run tail
touch /var/log/cron.log

# --- Start Cron and Log Output ---
echo "Starting cron scheduler..."
cron && tail -f /var/log/cron.log
