// مسیر فایل: src/IdentityService/Core/Entities/User.cs
namespace Core.Entities;

/// <summary>
/// نماینده یک کاربر در سیستم.
/// این کلاس یک Plain Old CLR Object (POCO) است و هیچ وابستگی به فریم‌ورک خاصی ندارد.
/// </summary>
public class User
{
    // شناسه منحصر به فرد کاربر. از Guid برای جلوگیری از حدس زدن شناسه‌ها استفاده می‌کنیم.
    public Guid Id { get; set; }

    // نام کاربری که برای ورود استفاده می‌شود. باید منحصر به فرد باشد.
    public string Username { get; set; } = string.Empty;

    // ایمیل کاربر که برای ارتباط و بازیابی رمز عبور استفاده می‌شود. باید منحصر به فرد باشد.
    public string Email { get; set; } = string.Empty;

    // هش رمز عبور. هرگز رمز عبور را به صورت متن ساده ذخیره نمی‌کنیم.
    public string PasswordHash { get; set; } = string.Empty;

    // تاریخ و زمان ایجاد حساب کاربری.
    public DateTime CreatedAt { get; set; }

    // تاریخ و زمان آخرین به‌روزرسانی پروفایل.
    public DateTime UpdatedAt { get; set; }
}

