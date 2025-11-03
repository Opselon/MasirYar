// src/IdentityService/Application/Contracts/IJwtTokenGenerator.cs
using Core.Entities;

namespace IdentityService.Application.Contracts;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}
