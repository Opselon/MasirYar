using IdentityService.Application.Contracts;
using Application.UseCases.UserRegistration;
using Core.Entities;
using Moq;
using Xunit;

namespace IdentityService.UnitTests.UseCases;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;

    public RegisterUserCommandHandlerTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _userRepositoryMock = new Mock<IUserRepository>();
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenInputIsValid()
    {
        // Arrange
        _passwordHasherMock.Setup(x => x.HashPassword("Password123")).Returns("hashed.password");
        var handler = new RegisterUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "testuser", Email = "test@example.com", Password = "Password123" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        _userRepositoryMock.Verify(x => x.AddAsync(It.Is<User>(u => u.Username == "testuser"), CancellationToken.None), Times.Once);
        _userRepositoryMock.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUsernameAlreadyExists()
    {
        // Arrange
        var existingUser = new User { Id = Guid.NewGuid(), Username = "testuser", Email = "existing@example.com", PasswordHash = "hashed.password" };
        _userRepositoryMock.Setup(x => x.FindByUsernameOrEmailAsync("testuser", "new@example.com", CancellationToken.None)).ReturnsAsync(existingUser);
        var handler = new RegisterUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "testuser", Email = "new@example.com", Password = "Password123" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = new User { Id = Guid.NewGuid(), Username = "existinguser", Email = "test@example.com", PasswordHash = "hashed.password" };
        _userRepositoryMock.Setup(x => x.FindByUsernameOrEmailAsync("newuser", "test@example.com", CancellationToken.None)).ReturnsAsync(existingUser);
        var handler = new RegisterUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "newuser", Email = "test@example.com", Password = "Password123" };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallPasswordHasher_WithCorrectPassword()
    {
        // Arrange
        var handler = new RegisterUserCommandHandler(_userRepositoryMock.Object, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "testuser", Email = "test@example.com", Password = "Password123" };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasherMock.Verify(x => x.HashPassword("Password123"), Times.Once);
    }
}
