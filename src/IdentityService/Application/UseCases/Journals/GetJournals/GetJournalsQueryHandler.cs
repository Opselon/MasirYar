// مسیر: src/IdentityService/Application/UseCases/Journals/GetJournals/GetJournalsQueryHandler.cs

using Application.Interfaces;
using MediatR;

namespace Application.UseCases.Journals.GetJournals;

/// <summary>
/// Handler برای پردازش GetJournalsQuery.
/// این کلاس منطق کسب‌وکار دریافت لیست ژورنال‌ها را پیاده‌سازی می‌کند.
/// </summary>
public class GetJournalsQueryHandler : IRequestHandler<GetJournalsQuery, List<JournalDto>>
{
    private readonly IJournalRepository _journalRepository;

    public GetJournalsQueryHandler(IJournalRepository journalRepository)
    {
        _journalRepository = journalRepository;
    }

    public async Task<List<JournalDto>> Handle(GetJournalsQuery request, CancellationToken cancellationToken)
    {
        var journals = await _journalRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return journals.Select(j => new JournalDto
        {
            Id = j.Id,
            Title = j.Title,
            Content = j.Content,
            CreatedAt = j.CreatedAt,
            UpdatedAt = j.UpdatedAt
        }).ToList();
    }
}

