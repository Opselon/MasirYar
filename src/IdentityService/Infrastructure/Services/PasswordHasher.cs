// مسیر: src/IdentityService/Infrastructure/Services/PasswordHasher.cs

using Application.Interfaces;

namespace Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس هش کردن رمز عبور با استفاده از BCrypt
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("Password cannot be null or empty.", nameof(password));
        }

        // استفاده از BCrypt برای هش کردن رمز عبور
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(passwordHash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
        catch
        {
            return false;
        }
    }
}

