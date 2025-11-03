using System.Net;
using System.Net.Http.Json;
using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CoachingService.IntegrationTests.Endpoints;

public class CourseEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public CourseEndpointsTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateCourse_ShouldReturnCreated_WhenPayloadIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var courseDto = new { Title = "Test Course", Description = "Test Description", AuthorId = "test_author" };

        // Act
        var response = await client.PostAsJsonAsync("/api/courses", courseDto);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }
}
