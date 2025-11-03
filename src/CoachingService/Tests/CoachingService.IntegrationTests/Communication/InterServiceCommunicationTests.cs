using System.Net;
using System.Net.Http.Json;
using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using Xunit;

namespace CoachingService.IntegrationTests.Communication;

public class InterServiceCommunicationTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly WireMockServer _identityServiceMock;

    public InterServiceCommunicationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
        _identityServiceMock = WireMockServer.Start();
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync()
    {
        _identityServiceMock.Stop();
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetCoursesEndpoint_ShouldReturnAuthorNames_WhenIdentityServiceIsAvailable()
    {
        // Arrange
        _identityServiceMock
            .Given(Request.Create().WithPath("/api/users/test_author").UsingGet())
            .RespondWith(Response.Create()
                .WithStatusCode(HttpStatusCode.OK)
                .WithBodyAsJson(new { Name = "Test Author" }));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["IdentityService:Url"] = _identityServiceMock.Urls[0]
                    }).Build());
            });
        }).CreateClient();

        var courseDto = new { Title = "Test Course", Description = "Test Description", AuthorId = "test_author" };
        await client.PostAsJsonAsync("/api/courses", courseDto);

        // Act
        var response = await client.GetAsync("/api/courses");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("Test Author", content);
    }
}
