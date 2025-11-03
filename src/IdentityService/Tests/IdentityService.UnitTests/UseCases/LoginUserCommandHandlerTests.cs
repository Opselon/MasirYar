using IdentityService.Application.Contracts;
using Application.UseCases.UserLogin;
using Core.Entities;
using Moq;
using Xunit;

namespace IdentityService.UnitTests.UseCases;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public LoginUserCommandHandlerTests()
    {
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
    }

    [Fact]
    public async Task Handle_ShouldReturnLoginResult_WhenCredentialsAreValid()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com", PasswordHash = "hashed.password" };
        _userRepositoryMock.Setup(x => x.FindByUsernameAsync("testuser", CancellationToken.None)).ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword("Password123", "hashed.password")).Returns(true);
        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("mock_jwt_token");

        var handler = new LoginUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object);
        var command = new LoginUserCommand { Username = "testuser", Password = "Password123" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("mock_jwt_token", result.Token);
        _jwtTokenGeneratorMock.Verify(x => x.GenerateToken(user), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenPasswordIsInvalid()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Username = "testuser", Email = "test@example.com", PasswordHash = "hashed.password" };
        _userRepositoryMock.Setup(x => x.FindByUsernameAsync("testuser", CancellationToken.None)).ReturnsAsync(user);

        _passwordHasherMock.Setup(x => x.VerifyPassword("WrongPassword", "hashed.password")).Returns(false);

        var handler = new LoginUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object, _jwtTokenGeneratorMock.Object);
        var command = new LoginUserCommand { Username = "testuser", Password = "WrongPassword" };

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(command, CancellationToken.None));
    }
}
