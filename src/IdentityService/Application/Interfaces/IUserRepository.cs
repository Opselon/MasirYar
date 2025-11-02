// مسیر: src/IdentityService/Application/Interfaces/IUserRepository.cs

using Core.Entities;

namespace Application.Interfaces;

/// <summary>
/// رابط (Interface) برای دسترسی به داده‌های کاربران
/// این Interface در لایه Application تعریف شده تا از وابستگی به Infrastructure جلوگیری شود.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// بررسی وجود کاربر با نام کاربری یا ایمیل مشخص
    /// </summary>
    Task<User?> FindByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default);

    /// <summary>
    /// یافتن کاربر بر اساس نام کاربری
    /// </summary>
    Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default);

    /// <summary>
    /// یافتن کاربر بر اساس شناسه
    /// </summary>
    Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت چند کاربر بر اساس لیست شناسه‌ها (Batch Get)
    /// این متد برای حل مشکل N+1 Query استفاده می‌شود
    /// </summary>
    Task<List<User>> FindByIdsAsync(List<Guid> userIds, CancellationToken cancellationToken = default);

    /// <summary>
    /// افزودن کاربر جدید به دیتابیس
    /// </summary>
    Task AddAsync(User user, CancellationToken cancellationToken = default);

    /// <summary>
    /// افزودن یک پیام Outbox جدید
    /// </summary>
    Task AddOutboxMessageAsync(OutboxMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره تغییرات در دیتابیس
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

