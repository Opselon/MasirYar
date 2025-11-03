// مسیر: src/IdentityService/Application/Interfaces/IJwtTokenGenerator.cs

namespace Application.Interfaces;

/// <summary>
/// رابط (Interface) برای تولید JWT Token
/// این Interface در لایه Application تعریف شده تا از وابستگی به Infrastructure جلوگیری شود.
/// </summary>
public interface IJwtTokenGenerator
{
    /// <summary>
    /// تولید JWT Token برای کاربر
    /// </summary>
    /// <param name="userId">شناسه کاربر</param>
    /// <param name="username">نام کاربری</param>
    /// <returns>JWT Token به صورت رشته</returns>
    string GenerateToken(Guid userId, string username);
}

