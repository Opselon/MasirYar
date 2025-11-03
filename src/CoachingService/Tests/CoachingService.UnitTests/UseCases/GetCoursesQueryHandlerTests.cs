using CoachingService.Application.UseCases.GetCourses;
using CoachingService.Core.Entities;
using CoachingService.Infrastructure.Persistence;
using CoachingService.Application.Contracts;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace CoachingService.UnitTests.UseCases;

public class GetCoursesQueryHandlerTests
{
    private readonly CoachingDbContext _dbContext;
    private readonly Mock<IUserClient> _userClientMock;

    public GetCoursesQueryHandlerTests()
    {
        var options = new DbContextOptionsBuilder<CoachingDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new CoachingDbContext(options);
        _userClientMock = new Mock<IUserClient>();
    }

    [Fact]
    public async Task Handle_ShouldCallUserClient_ToEnrichAuthorData()
    {
        // Arrange
        var course = new Course { Id = Guid.NewGuid(), Title = "Test Course", Description = "Test Description", AuthorId = "test_author" };
        await _dbContext.Courses.AddAsync(course);
        await _dbContext.SaveChangesAsync();

        _userClientMock.Setup(x => x.GetUserByIdAsync("test_author")).ReturnsAsync(new UserDto { Name = "Test Author" });

        var handler = new GetCoursesQueryHandler(_dbContext, _userClientMock.Object);
        var query = new GetCoursesQuery();

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Single(result);
        Assert.Equal("Test Author", result.First().AuthorName);
        _userClientMock.Verify(x => x.GetUserByIdAsync("test_author"), Times.Once);
    }
}
