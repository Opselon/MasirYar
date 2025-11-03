# /ops/backup/Dockerfile

# --- Stage 1: Build Environment ---
FROM python:3.11-slim as builder

WORKDIR /app

# Install system dependencies, including cron
RUN apt-get update && apt-get install -y --no-install-recommends \
    postgresql-client \
    redis-tools \
    cron \
    && rm -rf /var/lib/apt/lists/*

# Copy requirements and install python packages
COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

# Copy source code
COPY src/ ./src
COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

# --- Stage 2: Final Image ---
FROM python:3.11-slim

WORKDIR /app

# Copy system utilities and cron from builder stage
COPY --from=builder /usr/bin/pg_dump /usr/bin/
COPY --from=builder /usr/bin/psql /usr/bin/
COPY --from=builder /usr/bin/redis-cli /usr/bin/
COPY --from=builder /usr/sbin/cron /usr/sbin/
COPY --from=builder /etc/cron.d /etc/cron.d

# Copy installed Python packages
COPY --from=builder /usr/local/lib/python3.11/site-packages/ /usr/local/lib/python3.11/site-packages/

# Copy source code and entrypoint
COPY src/ ./src
COPY entrypoint.sh .

# Set the entrypoint to our script
ENTRYPOINT ["./entrypoint.sh"]
