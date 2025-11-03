using Application.Contracts;
using Application.UseCases.UserRegistration;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace IdentityService.UnitTests.UseCases;

public class RegisterUserCommandHandlerTests
{
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly IdentityDbContext _dbContext;

    public RegisterUserCommandHandlerTests()
    {
        _passwordHasherMock = new Mock<IPasswordHasher>();
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new IdentityDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenInputIsValid()
    {
        // Arrange
        _passwordHasherMock.Setup(x => x.HashPassword("Password123")).Returns("hashed.password");
        var handler = new RegisterUserCommandHandler(_dbContext, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "testuser", Email = "test@example.com", Password = "Password123" };

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, result);
        var user = await _dbContext.Users.FindAsync(result);
        Assert.NotNull(user);
        Assert.Equal("testuser", user.Username);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUsernameAlreadyExists()
    {
        // Arrange
        var existingUser = User.Create("testuser", "existing@example.com", "hashed.password");
        await _dbContext.Users.AddAsync(existingUser);
        await _dbContext.SaveChangesAsync();

        var handler = new RegisterUserCommandHandler(_dbContext, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "testuser", Email = "new@example.com", Password = "Password123" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenEmailAlreadyExists()
    {
        // Arrange
        var existingUser = User.Create("existinguser", "test@example.com", "hashed.password");
        await _dbContext.Users.AddAsync(existingUser);
        await _dbContext.SaveChangesAsync();

        var handler = new RegisterUserCommandHandler(_dbContext, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "newuser", Email = "test@example.com", Password = "Password123" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ShouldCallPasswordHasher_WithCorrectPassword()
    {
        // Arrange
        var handler = new RegisterUserCommandHandler(_dbContext, _passwordHasherMock.Object);
        var command = new RegisterUserCommand { Username = "testuser", Email = "test@example.com", Password = "Password123" };

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        _passwordHasherMock.Verify(x => x.HashPassword("Password123"), Times.Once);
    }
}
