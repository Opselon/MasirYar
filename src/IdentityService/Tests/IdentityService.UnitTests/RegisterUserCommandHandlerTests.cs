// src/IdentityService/Tests/IdentityService.UnitTests/RegisterUserCommandHandlerTests.cs
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.UseCases.UserRegistration;
using Application.Interfaces;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Moq;
using Infrastructure.Repositories;

namespace IdentityService.UnitTests;

public class RegisterUserCommandHandlerTests
{
    private IdentityDbContext GetInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<IdentityDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;
        return new IdentityDbContext(options);
    }

    [Fact]
    public async Task Handle_ShouldCreateUser_WhenUsernameAndEmailAreUnique()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var userRepository = new UserRepository(dbContext);
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var handler = new RegisterUserCommandHandler(userRepository, passwordHasherMock.Object);
        var command = new RegisterUserCommand
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "Password123"
        };
        passwordHasherMock.Setup(p => p.HashPassword(command.Password)).Returns("hashed_password");

        // Act
        var userId = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotEqual(Guid.Empty, userId);
        var userInDb = await dbContext.Users.FindAsync(userId);
        Assert.NotNull(userInDb);
        Assert.Equal("testuser", userInDb.Username);
    }

    [Fact]
    public async Task Handle_ShouldThrowException_WhenUsernameExists()
    {
        // Arrange
        var dbContext = GetInMemoryDbContext();
        var existingUser = new User { Id = Guid.NewGuid(), Username = "existinguser", Email = "unique@example.com", PasswordHash = "somehash" };
        await dbContext.Users.AddAsync(existingUser);
        await dbContext.SaveChangesAsync();

        var userRepository = new UserRepository(dbContext);
        var passwordHasherMock = new Mock<IPasswordHasher>();
        var handler = new RegisterUserCommandHandler(userRepository, passwordHasherMock.Object);
        var command = new RegisterUserCommand
        {
            Username = "existinguser", // Duplicate username
            Email = "new@example.com",
            Password = "Password123"
        };

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => handler.Handle(command, CancellationToken.None));
    }
}
