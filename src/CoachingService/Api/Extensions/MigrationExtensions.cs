// مسیر: src/CoachingService/Api/Extensions/MigrationExtensions.cs

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
        using IServiceScope scope = app.ApplicationServices.CreateScope();
        using CoachingDbContext dbContext = scope.ServiceProvider.GetRequiredService<CoachingDbContext>();
        
        try
        {
            Console.WriteLine("Attempting to apply migrations for CoachingService...");
            dbContext.Database.Migrate();
            Console.WriteLine("Migrations applied successfully for CoachingService.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred while applying migrations: {ex.Message}");
        }
    }
}

