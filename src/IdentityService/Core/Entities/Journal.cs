// مسیر فایل: src/IdentityService/Core/Entities/Journal.cs
namespace Core.Entities;

/// <summary>
/// نماینده یک ژورنال در سیستم.
/// این کلاس یک Plain Old CLR Object (POCO) است و هیچ وابستگی به فریم‌ورک خاصی ندارد.
/// </summary>
public class Journal
{
    // شناسه منحصر به فرد ژورنال
    public Guid Id { get; set; }

    // شناسه کاربری که این ژورنال متعلق به اوست
    public Guid UserId { get; set; }

    // عنوان ژورنال
    public string Title { get; set; } = string.Empty;

    // محتوای ژورنال
    public string Content { get; set; } = string.Empty;

    // تاریخ و زمان ایجاد ژورنال
    public DateTime CreatedAt { get; set; }

    // تاریخ و زمان آخرین به‌روزرسانی ژورنال
    public DateTime UpdatedAt { get; set; }

    // Navigation Property برای ارتباط با User (اختیاری، در صورت نیاز)
    // public User? User { get; set; }
}

