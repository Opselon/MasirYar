// مسیر: src/IdentityService/Application/Interfaces/IPasswordHasher.cs

namespace Application.Interfaces;

/// <summary>
/// رابط (Interface) برای سرویس هش کردن و تأیید رمز عبور
/// این Interface در لایه Application تعریف شده تا از وابستگی به Infrastructure جلوگیری شود.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// رمز عبور را هش می‌کند و رشته هش شده را برمی‌گرداند
    /// </summary>
    string HashPassword(string password);

    /// <summary>
    /// رمز عبور را با هش ذخیره شده مقایسه می‌کند
    /// </summary>
    /// <returns>true اگر رمز عبور صحیح باشد، در غیر این صورت false</returns>
    bool VerifyPassword(string password, string passwordHash);
}

