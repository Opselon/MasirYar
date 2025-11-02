// مسیر: src/IdentityService/Infrastructure/Repositories/UserRepository.cs

using Application.Interfaces;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

/// <summary>
/// پیاده‌سازی Repository برای دسترسی به داده‌های کاربران
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _dbContext;

    public UserRepository(IdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> FindByUsernameOrEmailAsync(string username, string email, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                u => u.Username == username || u.Email == email,
                cancellationToken);
    }

    public async Task<User?> FindByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(
                u => u.Username == username,
                cancellationToken);
    }

    public async Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
    }

    public async Task<List<User>> FindByIdsAsync(List<Guid> userIds, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Users
            .Where(u => userIds.Contains(u.Id))
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

