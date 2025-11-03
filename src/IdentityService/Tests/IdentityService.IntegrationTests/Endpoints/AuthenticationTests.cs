using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Api;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace IdentityService.IntegrationTests.Endpoints;

public class AuthenticationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthenticationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LoginEndpoint_ShouldReturnToken_ForExistingUser()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registrationDto = new { Username = "auth_test_user", Email = "auth@test.com", Password = "Password123" };
        await client.PostAsJsonAsync("/api/users/register", registrationDto);
        var loginDto = new { Username = "auth_test_user", Password = "Password123" };

        // Act
        var response = await client.PostAsJsonAsync("/api/users/login", loginDto);

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", content);
    }

    [Fact]
    public async Task ProtectedEndpoint_ShouldReturnUnauthorized_WhenNoTokenIsProvided()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/users/me");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ProtectedEndpoint_ShouldReturnOk_WhenValidTokenIsProvided()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registrationDto = new { Username = "token_test_user", Email = "token@test.com", Password = "Password123" };
        await client.PostAsJsonAsync("/api/users/register", registrationDto);
        var loginDto = new { Username = "token_test_user", Password = "Password123" };
        var loginResponse = await client.PostAsJsonAsync("/api/users/login", loginDto);
        var token = await loginResponse.Content.ReadAsStringAsync(); // This is a simplification

        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Replace("{\"token\":\"", "").Replace("\"}", ""));


        // Act
        var response = await client.GetAsync("/api/users/me");

        // Assert
        response.EnsureSuccessStatusCode();
    }
}
