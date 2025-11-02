// مسیر: src/CoachingService/Api/Controllers/CoursesController.cs

using Application.UseCases.Courses.GetCourses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CoursesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CoursesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetCourses()
    {
        try
        {
            var query = new GetCoursesQuery();
            var courses = await _mediator.Send(query);
            return Ok(courses);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Error = "An error occurred while retrieving courses." });
        }
    }
}

