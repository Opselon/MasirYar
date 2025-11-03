# /ops/backup/Dockerfile
FROM python:3.9-slim

WORKDIR /app

# Install PostgreSQL client for pg_dump and Redis tools for redis-cli
RUN apt-get update && apt-get install -y postgresql-client redis-tools && rm -rf /var/lib/apt/lists/*

COPY requirements.txt .
RUN pip install --no-cache-dir -r requirements.txt

COPY src/ .

COPY entrypoint.sh .
RUN chmod +x entrypoint.sh

ENTRYPOINT ["./entrypoint.sh"]
