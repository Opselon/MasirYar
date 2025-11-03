using Core.Entities;
using Xunit;

namespace IdentityService.UnitTests.Core.Entities;

public class UserEntityTests
{
    [Fact]
    public void Create_ShouldThrowException_WhenUsernameIsEmpty()
    {
        // Arrange, Act & Assert
        Assert.Throws<ArgumentException>(() => User.Create("", "test@example.com", "password"));
    }

    [Fact]
    public void ChangePassword_ShouldUpdatePasswordHash()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", "old_password");

        // Act
        user.ChangePassword("new_password");

        // Assert
        Assert.Equal("new_password", user.PasswordHash);
    }
}
