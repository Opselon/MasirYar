// مسیر: src/IdentityService/Application/UseCases/Journals/GetJournals/GetJournalsQuery.cs

using Core.Entities;
using MediatR;

namespace Application.UseCases.Journals.GetJournals;

/// <summary>
/// Query برای دریافت لیست ژورنال‌های یک کاربر.
/// این Query با استفاده از MediatR به Handler مربوطه ارسال می‌شود.
/// </summary>
public class GetJournalsQuery : IRequest<List<JournalDto>>
{
    public Guid UserId { get; set; }
}

/// <summary>
/// DTO برای نمایش اطلاعات ژورنال
/// </summary>
public class JournalDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

