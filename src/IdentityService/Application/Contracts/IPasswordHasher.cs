// src/IdentityService/Application/Contracts/IPasswordHasher.cs
namespace IdentityService.Application.Contracts;

public interface IPasswordHasher
{
    string HashPassword(string password);
    bool VerifyPassword(string hashedPasswordWithSalt, string password);
}
