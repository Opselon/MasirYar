// مسیر: src/IdentityService/Application/Interfaces/IJournalAnalyzer.cs

namespace Application.Interfaces;

/// <summary>
/// رابط (Interface) برای تحلیل ژورنال‌ها
/// این Interface در لایه Application تعریف شده تا از وابستگی به Infrastructure جلوگیری شود.
/// </summary>
public interface IJournalAnalyzer
{
    /// <summary>
    /// تحلیل محتوای یک ژورنال به صورت ناهمگام
    /// </summary>
    /// <param name="journalId">شناسه ژورنال</param>
    /// <param name="content">محتوای ژورنال برای تحلیل</param>
    Task AnalyzeJournalAsync(Guid journalId, string content);
}

