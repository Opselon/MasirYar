// مسیر: src/IdentityService/Infrastructure/Repositories/JournalRepository.cs

using Application.Interfaces;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository برای دسترسی به داده‌های ژورنال‌ها
/// </summary>
public class JournalRepository : IJournalRepository
{
    private readonly IdentityDbContext _dbContext;

    public JournalRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Journal journal, CancellationToken cancellationToken = default)
    {
        await _dbContext.Journals.AddAsync(journal, cancellationToken);
    }

    public async Task<List<Journal>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Journals
            .Where(j => j.UserId == userId)
            .OrderByDescending(j => j.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Journal?> GetByIdAsync(Guid journalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Journals
            .FirstOrDefaultAsync(j => j.Id == journalId, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

