// مسیر: src/IdentityService/Application/UseCases/Journals/CreateJournal/CreateJournalCommand.cs

using MediatR;

namespace Application.UseCases.Journals.CreateJournal;

/// <summary>
/// Command برای ایجاد ژورنال جدید.
/// این Command با استفاده از MediatR به Handler مربوطه ارسال می‌شود.
/// </summary>
public class CreateJournalCommand : IRequest<Guid>
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}

