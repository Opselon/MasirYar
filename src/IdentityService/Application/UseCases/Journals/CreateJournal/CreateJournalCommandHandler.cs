// مسیر: src/IdentityService/Application/UseCases/Journals/CreateJournal/CreateJournalCommandHandler.cs

using IdentityService.Application.Contracts;
using Core.Entities;
using Hangfire;
using MediatR;

namespace Application.UseCases.Journals.CreateJournal;

/// <summary>
/// Handler برای پردازش CreateJournalCommand.
/// این کلاس منطق کسب‌وکار ایجاد ژورنال را پیاده‌سازی می‌کند.
/// </summary>
public class CreateJournalCommandHandler : IRequestHandler<CreateJournalCommand, Guid>
{
    private readonly IJournalRepository _journalRepository;
    private readonly IBackgroundJobClient _backgroundJobClient;

    public CreateJournalCommandHandler(
        IJournalRepository journalRepository,
        IBackgroundJobClient backgroundJobClient)
    {
        _journalRepository = journalRepository;
        _backgroundJobClient = backgroundJobClient;
    }

    public async Task<Guid> Handle(CreateJournalCommand request, CancellationToken cancellationToken)
    {
        var journal = new Journal
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _journalRepository.AddAsync(journal, cancellationToken);
        await _journalRepository.SaveChangesAsync(cancellationToken);

        // ایجاد یک کار پس‌زمینه برای تحلیل این ژورنال
        _backgroundJobClient.Enqueue<IJournalAnalyzer>(analyzer => 
            analyzer.AnalyzeJournalAsync(journal.Id, journal.Content));

        return journal.Id;
    }
}

