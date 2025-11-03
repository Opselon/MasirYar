using CoachingService.Application.UseCases.CourseCreation;
using CoachingService.Core.Entities;
using CoachingService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoachingService.UnitTests.UseCases;

public class CreateCourseCommandHandlerTests
{
    private readonly CoachingDbContext _dbContext;

    public CreateCourseCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CoachingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new CoachingDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldCreateCourse_WhenInputIsValid()
    {
        // Arrange
        var handler = new CreateCourseCommandHandler(_dbContext);
        var command = new CreateCourseCommand { Title = "Test Course", Description = "Test Description", AuthorId = "test_author" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        var course = await _dbContext.Courses.FindAsync(result);
        Assert.NotNull(course);
        Assert.Equal("Test Course", course.Title);
    }
}
