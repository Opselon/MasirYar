// مسیر: src/IdentityService/Application/Interfaces/IJournalRepository.cs

using Core.Entities;

namespace Application.Interfaces;

/// <summary>
/// رابط (Interface) برای دسترسی به داده‌های ژورنال‌ها
/// این Interface در لایه Application تعریف شده تا از وابستگی به Infrastructure جلوگیری شود.
/// </summary>
public interface IJournalRepository
{
    /// <summary>
    /// افزودن ژورنال جدید به دیتابیس
    /// </summary>
    Task AddAsync(Journal journal, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت تمام ژورنال‌های یک کاربر
    /// </summary>
    Task<List<Journal>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// دریافت یک ژورنال بر اساس شناسه
    /// </summary>
    Task<Journal?> GetByIdAsync(Guid journalId, CancellationToken cancellationToken = default);

    /// <summary>
    /// ذخیره تغییرات در دیتابیس
    /// </summary>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

