using Application.Contracts;
using Application.UseCases.UserLogin;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace IdentityService.UnitTests.UseCases;

public class LoginUserCommandHandlerTests
{
    private readonly Mock<IJwtTokenGenerator> _jwtTokenGeneratorMock;
    private readonly IdentityDbContext _dbContext;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;

    public LoginUserCommandHandlerTests()
    {
        _jwtTokenGeneratorMock = new Mock<IJwtTokenGenerator>();
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new IdentityDbContext(options);
        _passwordHasherMock = new Mock<IPasswordHasher>();
    }

    [Fact]
    public async Task Handle_ShouldReturnLoginResult_WhenCredentialsAreValid()
    {
        // Arrange
        var user = User.Create("testuser", "test@example.com", "hashed.password");
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        _passwordHasherMock.Setup(x => x.VerifyPassword("hashed.password", "Password123")).Returns(true);
        _jwtTokenGeneratorMock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("mock_jwt_token");

        var handler = new LoginUserCommandHandler(_dbContext, _jwtTokenGeneratorMock.Object, _passwordHasherMock.Object);
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
        var user = User.Create("testuser", "test@example.com", "hashed.password");
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();

        _passwordHasherMock.Setup(x => x.VerifyPassword("hashed.password", "WrongPassword")).Returns(false);

        var handler = new LoginUserCommandHandler(_dbContext, _jwtTokenGeneratorMock.Object, _passwordHasherMock.Object);
        var command = new LoginUserCommand { Username = "testuser", Password = "WrongPassword" };

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(() => handler.Handle(command, CancellationToken.None));
    }
}
