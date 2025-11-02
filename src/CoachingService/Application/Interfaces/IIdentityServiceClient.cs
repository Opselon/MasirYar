// مسیر: src/CoachingService/Application/Interfaces/IIdentityServiceClient.cs

namespace Application.Interfaces;

/// <summary>
/// رابط برای ارتباط با IdentityService
/// این Interface در لایه Application تعریف شده تا از وابستگی مستقیم جلوگیری شود.
/// </summary>
public interface IIdentityServiceClient
{
    /// <summary>
    /// دریافت اطلاعات یک کاربر بر اساس شناسه
    /// </summary>
    Task<UserInfo?> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت اطلاعات چند کاربر به صورت Batch (برای حل مشکل N+1 Query)
    /// </summary>
    Task<Dictionary<Guid, UserInfo>> GetUsersByIdsAsync(List<Guid> userIds, CancellationToken cancellationToken = default);
}

/// <summary>
/// DTO برای اطلاعات کاربر از IdentityService
/// این کلاس برای استفاده در Application layer تعریف شده است.
/// </summary>
public class UserInfo
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

