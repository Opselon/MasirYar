// مسیر: src/CoachingService/Application/UseCases/Courses/GetCourses/GetCoursesQueryHandler.cs

using Application.Interfaces;
using MediatR;

namespace Application.UseCases.Courses.GetCourses;

/// <summary>
/// Handler برای GetCoursesQuery با استفاده از Batch Get برای حل مشکل N+1 Query
/// </summary>
public class GetCoursesQueryHandler : IRequestHandler<GetCoursesQuery, List<CourseDto>>
{
    private readonly ICourseRepository _courseRepository;
    private readonly IIdentityServiceClient _identityServiceClient;

    public GetCoursesQueryHandler(
        ICourseRepository courseRepository,
        IIdentityServiceClient identityServiceClient)
    {
        _courseRepository = courseRepository;
        _identityServiceClient = identityServiceClient;
    }

    public async Task<List<CourseDto>> Handle(GetCoursesQuery request, CancellationToken ct)
    {
        // ۱. دریافت تمام دوره‌ها از دیتابیس
        var courses = await _courseRepository.GetAllAsync(ct);

        if (!courses.Any())
        {
            return new List<CourseDto>();
        }

        // ۲. استخراج تمام AuthorId های منحصر به فرد
        var authorIds = courses
            .Select(c => c.AuthorId)
            .Distinct()
            .ToList();

        // ۳. دریافت اطلاعات تمام نویسندگان در یک درخواست Batch (حل مشکل N+1)
        var authors = await _identityServiceClient.GetUsersByIdsAsync(authorIds, ct);

        // ۴. تبدیل Course ها به CourseDto با استفاده از Dictionary (O(1) lookup)
        var courseDtos = courses.Select(course =>
        {
            var author = authors.GetValueOrDefault(course.AuthorId);
            return new CourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                AuthorId = course.AuthorId,
                AuthorName = author?.Username ?? "Unknown"
            };
        }).ToList();

        return courseDtos;
    }
}

