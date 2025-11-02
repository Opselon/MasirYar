// مسیر: src/CoachingService/Application/UseCases/Courses/GetCourses/GetCoursesQuery.cs

using MediatR;

namespace Application.UseCases.Courses.GetCourses;

/// <summary>
/// Query برای دریافت لیست دوره‌ها
/// </summary>
public class GetCoursesQuery : IRequest<List<CourseDto>>
{
}

/// <summary>
/// DTO برای نمایش اطلاعات دوره
/// </summary>
public class CourseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
}

