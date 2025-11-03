using Core.Entities;
using Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace IdentityService.UnitTests.Infrastructure.Services;

public class JwtTokenGeneratorTests
{
    private readonly IConfiguration _configuration;

    public JwtTokenGeneratorTests()
    {
        var inMemorySettings = new Dictionary<string, string> {
            {"Jwt:SecretKey", "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!"},
            {"Jwt:Issuer", "TestIssuer"},
            {"Jwt:Audience", "TestAudience"},
            {"Jwt:ExpirationMinutes", "60"},
        };

        _configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(inMemorySettings)
            .Build();
    }

    [Fact]
    public void GenerateToken_ShouldContainCorrectUserIdClaim()
    {
        // Arrange
        var tokenGenerator = new JwtTokenGenerator(_configuration);
        var user = User.Create("testuser", "test@example.com", "password");

        // Act
        var token = tokenGenerator.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(token);

        // Assert
        var userIdClaim = decodedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
        Assert.NotNull(userIdClaim);
        Assert.Equal(user.Id.ToString(), userIdClaim.Value);
    }

    [Fact]
    public void GenerateToken_ShouldHaveCorrectIssuerAndAudience()
    {
        // Arrange
        var tokenGenerator = new JwtTokenGenerator(_configuration);
        var user = User.Create("testuser", "test@example.com", "password");

        // Act
        var token = tokenGenerator.GenerateToken(user);
        var handler = new JwtSecurityTokenHandler();
        var decodedToken = handler.ReadJwtToken(token);

        // Assert
        Assert.Equal("TestIssuer", decodedToken.Issuer);
        Assert.Equal("TestAudience", decodedToken.Audiences.FirstOrDefault());
    }
}
