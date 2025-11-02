// مسیر فایل: src/IdentityService/Infrastructure/Persistence/IdentityDbContext.cs
using Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Journal> Journals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // تعریف ایندکس‌های منحصر به فرد برای جلوگیری از تکرار نام کاربری و ایمیل در سطح دیتابیس
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();
        
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // پیکربندی Journal
        modelBuilder.Entity<Journal>()
            .HasIndex(j => j.UserId);

        // تعریف رابطه بین Journal و User (اختیاری)
        modelBuilder.Entity<Journal>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(j => j.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

