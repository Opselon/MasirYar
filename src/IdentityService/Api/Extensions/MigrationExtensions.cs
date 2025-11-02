// مسیر فایل: src/IdentityService/Api/Extensions/MigrationExtensions.cs

using Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;

namespace Api.Extensions;

/// <summary>
/// کلاس کمکی برای اعمال خودکار Migration‌ها در زمان راه‌اندازی برنامه.
/// </summary>
public static class MigrationExtensions
{
    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        // یک scope جدید برای دریافت سرویس‌ها ایجاد می‌کنیم.
        // دلیل: IApplicationBuilder یک Singleton است ولی DbContext یک Scoped service است.
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        
        using IdentityDbContext dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();
        
        try
        {
            Console.WriteLine("Attempting to apply migrations...");
            // دستور اصلی برای اجرای Migration های اعمال نشده.
            dbContext.Database.Migrate();
            Console.WriteLine("Migrations applied successfully.");
        }
        catch (Exception ex)
        {
            // در صورت بروز خطا، آن را لاگ می‌گیریم تا در لاگ‌های داکر قابل مشاهده باشد.
            Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
            // در محیط پروداکشن، بهتر است اینجا برنامه را متوقف کنید یا سیاست‌های دیگری در پیش بگیرید.
        }
    }
}

