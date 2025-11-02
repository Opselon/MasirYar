// مسیر: src/CoachingService/Infrastructure/Repositories/CourseRepository.cs

using Application.Interfaces;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CourseRepository : ICourseRepository
{
    private readonly CoachingDbContext _dbContext;

    public CourseRepository(CoachingDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Courses
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);
    }

    public async Task AddAsync(Course course, CancellationToken cancellationToken = default)
    {
        await _dbContext.Courses.AddAsync(course, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}

