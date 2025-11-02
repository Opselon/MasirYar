// مسیر: src/IdentityService/Infrastructure/Services/JwtTokenGenerator.cs

using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Infrastructure.Services;

/// <summary>
/// پیاده‌سازی سرویس تولید JWT Token
/// </summary>
public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly string _secretKey;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _expirationMinutes;

    public JwtTokenGenerator(IConfiguration configuration)
    {
        _secretKey = configuration["Jwt:SecretKey"] 
            ?? throw new InvalidOperationException("JWT SecretKey is not configured.");
        _issuer = configuration["Jwt:Issuer"] ?? "IdentityService";
        _audience = configuration["Jwt:Audience"] ?? "IdentityService";
        _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"] ?? "60");
    }

    public string GenerateToken(Guid userId, string username)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Name, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_expirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

