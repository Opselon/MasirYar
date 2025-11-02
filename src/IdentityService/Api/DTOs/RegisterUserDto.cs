// مسیر: src/IdentityService/Api/DTOs/RegisterUserDto.cs

namespace Api.DTOs;

/// <summary>
/// Data Transfer Object برای ثبت‌نام کاربر جدید
/// </summary>
public class RegisterUserDto
{
    /// <summary>
    /// نام کاربری که باید منحصر به فرد باشد
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// آدرس ایمیل کاربر که باید منحصر به فرد باشد
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// رمز عبور کاربر (قبل از ارسال به سرور)
    /// </summary>
    public string Password { get; set; } = string.Empty;
}

