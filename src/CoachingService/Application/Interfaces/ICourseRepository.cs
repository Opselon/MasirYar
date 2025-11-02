// مسیر: src/CoachingService/Application/Interfaces/ICourseRepository.cs

using Core.Entities;

namespace Application.Interfaces;

/// <summary>
/// رابط برای دسترسی به داده‌های دوره‌ها
/// </summary>
public interface ICourseRepository
{
    Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Course?> GetByIdAsync(Guid courseId, CancellationToken cancellationToken = default);
    Task AddAsync(Course course, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}

