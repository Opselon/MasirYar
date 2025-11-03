<div align="center">
  <img src="https://raw.githubusercontent.com/user-attachments/assets/dd38f870-136b-4e1d-8557-d318e3e4a3b8/logo.png" alt="MasirYar Logo" width="140" />

# MasirYar — پلتفرم هوشمند رشد شخصی (Full README)

**نسخهٔ جامع، طراحی مهندسی‌محور و آموزشی — مناسب برای توسعه در Visual Studio 2022 و دیپلوی در لینوکس**

</div>

---

> **خلاصه:** این مخزن شامل یک پلتفرم میکروسرویس برای رشد شخصی است که با رویکرد Production-First و Clean Architecture طراحی شده. هدف ارائهٔ تجربهٔ قابل اتکا، مقیاس‌پذیر و توسعه‌پذیر برای سرویس‌های مبتنی بر داده و هوش مصنوعی است.

---

## 🔖 فهرست مطالب

1. [نکات کلیدی و هدف](#-نکات-کلیدی-و-هدف)
2. [ویژگی‌های برجسته](#-ویژگی‌های-برجسته)
3. [پیش‌نیازها و یک-دستور راه‌اندازی](#-پیش‌نیازها-و-یک-دستور-راه‌اندازی)
4. [ساختار پروژه و مسیر فایل‌ها (Paths)](#-ساختار-پروژه-و-مسیر-فایل‌ها-paths)
5. [نحوهٔ کار با Visual Studio 2022 (راهنمای توسعه‌دهنده)](#-نحوهٔ-کار-با-visual-studio-2022-راهنمای-توسعه‌دهنده)
6. [پکیج‌ها (NuGet & npm) و توضیح استفاده](#-پکیجها-nuget--npm-و-توضیح-استفاده)
7. [اسکریپت Setup Wizard — run.sh / run.ps1](#-اسکریپت-setup-wizard----runsh--runps1)
8. [حالت آموزشی (Tutorial / Learning Mode)](#-حالت-آموزشی-tutorial--learning-mode)
9. [مستندسازی API و OpenAPI (Swagger)](#-مستندسازی-api-و-openapi-swagger)
10. [تست‌ها، CI/CD و دیپلوی به لینوکس](#-تستها-cicd-و-دیپلوی-به-لینوکس)
11. [امنیت، بکاپ و مانیتورینگ](#-امنیت-بکاپ-و-مانیتورینگ)
12. [چک‌لیست آماده‌سازی برای ارائه (Resume/Portfolio)](#-چک‌لیست-آماده‌سازی-برای-ارائه-resumeportfolio)
13. [مشارکت و کانتریبیشن](#-مشارکت-و-کانتریبیشن)

---

## 🧭 نکات کلیدی و هدف

* **نام پروژه:** MasirYar (مسیریار)
* **هدف:** ارائهٔ یک پلتفرم SaaS برای رشد شخصی، پیگیری عادت‌ها، ژورنالینگ، مسیرهای توسعه و کوچینگ هوشمند با قابلیت RAG و embeddings.
* **رویکرد طراحی:** Clean Architecture + DDD + SOLID — با تمرکز روی قابلیت تست و توسعهٔ مستقل برای هر bounded context.
* **محیط توسعه:** Visual Studio 2022 (Windows) و Docker + Linux برای دیپلوی.

---

## ✨ ویژگی‌های برجسته

* میکروسرویس‌محور و Postgres محور (pgvector، JSONB، GIN, tsvector)
* RAG / Embeddings ذخیره‌شده در Postgres با pgvector
* Redis برای cache و صف پیام
* Hangfire برای اجرای jobهای پس‌زمینه (یا Quartz بر حسب انتخاب)
* Telegram Bot integration برای نوتیف و تعامل کاربران
* OpenAPI (Swashbuckle) برای مستندسازی API
* Observability: Serilog → Loki + Prometheus → Grafana

---

## 🔧 پیش‌نیازها و یک-دستور راه‌اندازی

**پیش‌نیاز اصلی:** Docker Desktop (یا Docker Engine و docker-compose روی لینوکس)

```bash
# کلون کردن repo
git clone https://github.com/Opselon/MasirYar.git
cd MasirYar

# در لینوکس/مک (اولین بار)
chmod +x run.sh
./run.sh up

# در ویندوز (PowerShell)
# Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\run.ps1 up
```

> اسکریپت راه‌انداز ('run.sh' / 'run.ps1') یک ویزارد تعاملی اجرا می‌کند که از شما **Connection String** و متغیرهای ضروری را می‌پرسد و فایل `.env` را می‌سازد سپس `docker-compose` را اجرا می‌کند.

---

## 📁 ساختار پروژه و مسیر فایل‌ها (Paths)

تمام مسیرها طوری طراحی شده‌اند که با Visual Studio 2022 سازگار باشند.
مسیرها کلیدی که باید بدانید:

```
/MasirYar
├─ /docker                 # فایل‌های docker, nginx, certs template
├─ /setup                  # run.sh, run.ps1, templates (.env.template)
├─ /src
│   ├─ /Domain
│   ├─ /Application
│   ├─ /Infrastructure
│   ├─ /WebAPI              # Project.Api (Presentation)
│   ├─ /BackgroundTasks
│   └─ /TelegramPanel
├─ /frontend               # Next.js app
├─ /docs                   # ERD, architecture diagrams, runbooks
└─ README.md
```

> **نکته مسیر دقیق فایل‌ها در Visual Studio 2022:** حل (Solution) در `src/MasirYar.sln` قرار دارد؛ پروژه‌ها تحت پوشهٔ `src/` هستند.
> کنترل‌کن: `src/Project.Api` برابر با `/WebAPI` است و `src/Project.Infrastructure` شامل `DbContext` و مایگریشن‌ها می‌باشد.

---

## 🛠️ نحوهٔ کار با Visual Studio 2022 (راهنمای توسعه‌دهنده)

1. باز کردن راه‌حل:

   * File → Open → Project/Solution → `src/MasirYar.sln`.
2. تنظیم Startup projects (برای توسعه محلی):

   * Project.Api (WebAPI) و BackgroundTasks و TelegramPanel را به عنوان چند استارت‌آپ انتخاب کن.
3. تنظیم connection string برای توسعه:

   * از `setup/.env.template` کپی کن و فایل `.env` بساز و مقادیر را پر کن.
   * یا در Visual Studio در Debug → Environment Variables مقادیر را اضافه کن.
4. اجرای مایگریشن‌های EF Core در VS2022 (Package Manager Console):

```powershell
# Tools -> NuGet Package Manager -> Package Manager Console
# تغییر به پروژه اینفر استراکتچر
PM> cd src/Project.Infrastructure
PM> dotnet ef migrations add InitialCreate -s ../Project.Api -p ./Project.Infrastructure
PM> dotnet ef database update -s ../Project.Api -p ./Project.Infrastructure
```

> **نکته:** برای اطمینان از سازگاری با VS2022، از .NET 9 SDK و نسخه‌های NuGet پیشنهادشده در بخش بعدی استفاده کن.

---

## 📦 پکیج‌ها (NuGet و npm) و توضیح کاربرد

در ادامه لیست پکیج‌های اصلی که در هر لایه استفاده می‌شوند و دلیل انتخابشان آمده است.

### NuGet (Backend / .NET 9)

* `Npgsql.EntityFrameworkCore.PostgreSQL` — EF Core provider برای Postgres (مایگریشن‌ها و LINQ)
* `Microsoft.EntityFrameworkCore.Design` — ابزارهای EF Core
* `MediatR` — الگوی CQRS/مدیریت دستورات و کوئری‌ها
* `AutoMapper.Extensions.Microsoft.DependencyInjection` — نگاشت DTO به Domain
* `Swashbuckle.AspNetCore` — Swagger/OpenAPI
* `Serilog.AspNetCore`, `Serilog.Sinks.File` — لاگینگ ساختاریافته
* `StackExchange.Redis` — کلاینت Redis
* `Hangfire.Core`, `Hangfire.PostgreSql` — scheduler / background jobs
* `Telegram.Bot` — ارتباط با Telegram API
* `pgvector-net` یا mapping سفارشی Npgsql — کار با pgvector
* `FluentValidation.AspNetCore` — اعتبارسنجی DTO ها
* `OpenTelemetry` / `OpenTelemetry.Exporter.Prometheus` — telemetry و metrics
* `Polly` — resilience و retry policies
* `xunit`, `Moq`, `FluentAssertions` — تست‌ها

### npm (Frontend / Next.js)

* `next`, `react`, `react-dom`, `typescript`
* `tailwindcss` — استایل مدرن و سریع
* `axios` — فراخوانی API
* `swr` یا `react-query` — مدیریت fetch و caching
* `recharts` یا `chart.js` — نمودارهای گزارش‌گیری
* `i18next` / `react-i18next` — پشتیبانی چندزبانه (fa-IR default)
* `playwright` — تست‌های E2E

---

## 🔁 اسکریپت Setup Wizard — `run.sh` و `run.ps1`

**محل فایل‌ها:** `/setup/run.sh` و `/setup/run.ps1` (ریشهٔ repo `/setup`)

**قابلیت‌ها:**

* تشخیص OS (Linux/macOS/Windows)
* بررسی نصب Docker/Docker Compose
* ویزارد تعاملی برای پر کردن `.env` (DB connection, JWT_SECRET, REDIS_PASSWORD, OPENAI_KEY, TELEGRAM_BOT_TOKEN)
* انتخاب حالتِ Online (pull images) یا Offline (build from local sources)
* اجرای `docker compose -f docker-compose.dev.yml up --build -d`
* ارائهٔ دستورات مفید پس از اجرا (healthcheck URLs)

**دستورات معمول:**

```bash
# برای راه‌اندازی همه سرویس‌ها
./run.sh up
# متوقف کردن
./run.sh down
# نمایش لاگ یک سرویس
./run.sh logs project-api
# اجرای تست‌ها
./run.sh test
# اجرای مجدد ویرایش پیکربندی
./run.sh config
```

> **امنیت:** اسکریپت از docker secrets برای نگهداری مقادیر حساس در production پشتیبانی می‌کند و توصیه می‌شود برای production استفاده شود.

---

## 🎓 حالت آموزشی (Tutorial / Learning Mode)

یک بخش آموزشی در پروژه گنجانده شده است تا توسعه‌دهنده یا دانشجو بتواند قدم‌به‌قدم با پروژه آشنا شود. این مد شامل:

1. **مقدمات معماری:** راهنمایی قدم‌به‌قدم در مورد لایه‌ها و نحوهٔ ارتباط آنها (README/docs/architecture.md)
2. **تمرین‌های عملی (Labs):** مجموعهٔ تمرینات در `docs/tutorials/` مثل:

   * Lab 01: اضافه کردن یک فیلد جدید به `User` و اعمال مایگریشن در EF Core
   * Lab 02: نوشتن Unit Test برای `JournalService`
   * Lab 03: اضافه کردن endpoint جدید و تست E2E با Playwright
3. **مسیر یادگیری (Learning Path):** فهرست مقالات، ویدیوها و منابع برای یادگیری
4. **تمرین استقرار:** راهنمای گام‌به‌گام برای deploy در یک VM لینوکسی عمومی (Ubuntu 22.04)

> هر Lab شامل: هدف، فایل‌های مرتبط، فرمان‌ها، و پاسخ مثال است؛ همچنین یک بخش "چک‌لیست" برای ارزیابی موفقیت دارد.

---

## 📚 نمونهٔ workflow توسعه (یک سناریو عملی)

**هدف:** توسعهٔ سریع یک API جدید `POST /api/journals` که ژورنال کاربر را ذخیره و job آنالیز AI را enqueue می‌کند.

### 1) تعریف Domain (مسیر فایل)

* Path: `src/Domain/Entities/Journal.cs`
* توضیح: تعریف کلاس Journal با فیلدهای `Id, UserId, Text, CreatedAt, MoodScore`.

### 2) Application Layer

* Path: `src/Application/Services/JournalService.cs`
* توضیح: متد `CreateJournalAsync(userId, dto)` که کار ذخیره‌سازی را انجام می‌دهد و `IBackgroundJobClient.Enqueue(() => AnalyzeJournal(jobId))` را فراخوانی می‌کند.

### 3) Infrastructure

* Path: `src/Infrastructure/Persistence/AppDbContext.cs`
* توضیح: DbSet<Journal> و تنظیمات pgvector/tsvector مربوطه.

### 4) WebAPI (Controller)

* Path: `src/WebAPI/Controllers/JournalController.cs`
* توضیح: Controller با متد `[HttpPost]` که DTO را validate کرده و JournalService را فراخوانی می‌کند. کامنت‌های فارسی توصیفی در بالای متد.

### 5) تست

* Path: `src/Project.Tests/JournalServiceTests.cs`
* توضیح: یک Unit Test برای اطمینان از اینکه Journal ذخیره و job ثبت می‌شود.

### 6) اجرا

```bash
# Run migrations
cd src/Project.Infrastructure
dotnet ef database update -s ../Project.Api -p ./Project.Infrastructure
# Run tests
./run.sh test
```

---

## 🧪 تست‌ها، CI/CD و دیپلوی به لینوکس

**CI/CD:** مثال GitHub Actions در `.github/workflows/ci.yml` شامل مراحل: build, test, docker build & push, و deploy (ssh to server + docker-compose pull & up).

**تست‌های پیشنهادی:**

* Unit tests (xUnit) برای Serviceها و Repositories
* Integration tests با Testcontainers (Postgres / Redis) برای آزمون واقعی DB
* E2E tests با Playwright برای workflows کاربر
* Load test با k6 یا Artillery برای endpoints حساس

**دیپلوی به لینوکس (پیشنهاد عملی):**

1. سرور: Ubuntu 22.04 LTS
2. نصب Docker + Docker Compose v2
3. انتقال فایل `docker-compose.prod.yml` و `.env` امن
4. اجرای `docker compose -f docker-compose.prod.yml up -d --build`
5. نصب systemd unit برای مانیتورینگ و autostart (دستورالعمل در `docs/runbooks/deploy.md`)

---

## 🔒 امنیت، بکاپ و مانیتورینگ

**امنیت:**

* Use Docker secrets / Vault for production secrets
* DB access: allow only private network access; use user-specific DB credentials
* HTTPS via nginx + certbot
* Input validation + rate limiting + CSP headers

**بکاپ:** شبانه `pg_dump` و نگهداری در S3-compatible storage یا remote backup

**مانیتورینگ و لاگ:**

* Serilog → Loki
* Metrics: Prometheus + Grafana
* Alerts: Grafana alerting on CPU, error rate, DB connections

---

## ✅ چک‌لیست آماده‌سازی برای ارائه (Resume / Portfolio)

این بخش کمک می‌کند تا پروژه‌ات به عنوان یک نمونهٔ کاری نمایش داده شود:

* ✅ README کامل و مستندسازی (همین فایل)
* ✅ Demo video (1–3 دقیقه) از محیط اجرا شده
* ✅ اسکرین‌شات dashboard و مثال‌های گزارش
* ✅ توضیح معماری و نمودار ERD در docs/
* ✅ لینک deploy (staging) و credentials نمونه

---

## 🤝 مشارکت و کانتریبیشن

* لطفاً قبل از ارسال PR، یک Issue مربوط به تغییر پیشنهادیت ایجاد کن.
* PR باید: شرح تغییرات (فارسی)، تست‌ها، و دستورالعمل اجرای محلی داشته باشد.

---

## 🔚 سخن پایانی

MasirYar یک پروژهٔ مهندسی‌شده، هدفمند و قابل توسعه‌ست که از ابتدا برای تولید و رشد طراحی شده.
در صورت نیاز، می‌توانم برایت:

* Skeleton کد (C#) + Dockerfile و docker-compose نمونه تولید کنم، یا
* مستقیم `run.sh` و `run.ps1` تعاملی را بسازم، یا
* بخش Tutorials/ Labs را با مثال‌های عملی پر کنم.

اگر آماده‌ای، بگو کدام خروجی را همین الآن بسازم: skeleton کد، اسکریپت نصب، یا مجموعهٔ Labs.
