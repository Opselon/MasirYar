// src/IdentityService/Tests/IdentityService.IntegrationTests/OutboxTests.cs
using System.Net.Http.Json;
using System.Threading.Tasks;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace IdentityService.IntegrationTests;

public class OutboxTests : IClassFixture<ApiFactory>
{
    private readonly ApiFactory _factory;

    public OutboxTests(ApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task RegisterUser_ShouldCreateOutboxMessage_WhenPayloadIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new
        {
            username = $"testuser_{Guid.NewGuid()}",
            email = $"test_{Guid.NewGuid()}@example.com",
            password = "Password123!"
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/users/register", request);

        // Assert
        response.EnsureSuccessStatusCode();

        // Verify the outbox message was created
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<IdentityDbContext>();

        var outboxMessage = await dbContext.OutboxMessages
            .FirstOrDefaultAsync(m => m.Type == "UserRegisteredEvent");

        Assert.NotNull(outboxMessage);
        Assert.Contains(request.username, outboxMessage.Content);
        Assert.Contains(request.email, outboxMessage.Content);
    }
}
