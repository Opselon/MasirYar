# تست‌های یکپارچه‌سازی IdentityService

این پوشه شامل تست‌های یکپارچه‌سازی برای `IdentityService` است. این تست‌ها از `Testcontainers` برای راه‌اندازی یک پایگاه داده PostgreSQL موقت استفاده می‌کنند تا از ایزوله بودن تست‌ها و عدم تأثیر آن‌ها بر روی پایگاه داده اصلی اطمینان حاصل شود.

## پیش‌نیازها

-   [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   [Docker](https://www.docker.com/products/docker-desktop)

## نحوه اجرای تست‌ها

برای اجرای تست‌ها، دستور زیر را در ریشه ریپازیتوری اجرا کنید:

```bash
dotnet test src/IdentityService/IdentityService.sln
```
