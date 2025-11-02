// src/NotificationService/Tests/IntegrationTests/UserRegisteredListenerTests.cs
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Api.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;
using Xunit;

namespace IntegrationTests;

public class NotificationWebApplicationFactory : WebApplicationFactory<Program>
{
    public Mock<ILogger<UserRegisteredListener>> MockLogger { get; } = new();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the real logger and replace it with our mock
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ILogger<UserRegisteredListener>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
            services.AddSingleton(MockLogger.Object);
        });
    }
}

public class UserRegisteredListenerTests : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();
    private readonly NotificationWebApplicationFactory _factory = new();

    [Fact]
    public async Task Should_LogWelcomeMessage_When_UserRegisteredEventIsReceived()
    {
        // Arrange
        _factory.MockLogger.Invocations.Clear();

        var client = _factory.CreateClient(); // This starts the hosted service

        var rabbitMqHost = _rabbitMqContainer.GetConnectionString();
        _factory.Services.GetRequiredService<IConfiguration>()["RabbitMq:HostName"] = rabbitMqHost;


        var connectionFactory = new ConnectionFactory { Uri = new Uri(rabbitMqHost) };
        using var connection = connectionFactory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Fanout);

        var userEvent = new {
            UserId = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@example.com"
        };
        var message = JsonSerializer.Serialize(userEvent);
        var body = Encoding.UTF8.GetBytes(message);

        // Act
        await Task.Delay(5000); // Allow time for the listener to connect

        channel.BasicPublish(
            exchange: "user_events",
            routingKey: "",
            basicProperties: null,
            body: body);

        await Task.Delay(2000); // Allow time for processing

        // Assert
        _factory.MockLogger.Verify(
            logger => logger.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Received UserRegisteredEvent for user testuser")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
            Times.Once);
    }

    public async Task InitializeAsync()
    {
        await _rabbitMqContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _rabbitMqContainer.StopAsync();
        await _factory.DisposeAsync();
    }
}
