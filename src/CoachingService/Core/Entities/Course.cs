// مسیر: src/CoachingService/Core/Entities/Course.cs
namespace Core.Entities;

/// <summary>
/// نماینده یک دوره آموزشی در سیستم
/// </summary>
public class Course
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; } // شناسه نویسنده دوره که از IdentityService می‌آید
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

