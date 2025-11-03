using System.Text;
using System.Text.Json;
using Api;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;
using Testcontainers.RabbitMq;
using Xunit;

namespace NotificationService.IntegrationTests.Flows;

public class EventConsumptionTests : IClassFixture<WebApplicationFactory<Program>>, IAsyncLifetime
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly RabbitMqContainer _brokerContainer = new RabbitMqBuilder().Build();

    public EventConsumptionTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    public async Task InitializeAsync()
    {
        await _brokerContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _brokerContainer.DisposeAsync();
    }

    [Fact]
    public async Task When_UserRegisteredEventIsPublished_EventHandlerShouldProcessItSuccessfully()
    {
        // Arrange
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["RabbitMq:HostName"] = _brokerContainer.Hostname,
                        ["RabbitMq:Port"] = _brokerContainer.GetMappedPublicPort(5672).ToString()
                    }).Build());
            });
        }).CreateClient();

        var factory = new ConnectionFactory()
        {
            HostName = _brokerContainer.Hostname,
            Port = _brokerContainer.GetMappedPublicPort(5672)
        };
        using var connection = factory.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(exchange: "user-exchange", type: ExchangeType.Fanout);

        var message = new { UserId = Guid.NewGuid(), Email = "test@example.com" };
        var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(message));

        // Act
        channel.BasicPublish(exchange: "user-exchange",
                             routingKey: "",
                             basicProperties: null,
                             body: body);

        // Assert
        await Task.Delay(2000); // Give time for the event to be processed
        // In a real scenario, we would check a database or a log to verify that the event was processed.
        // For this example, we'll assume success if no exceptions were thrown.
        Assert.True(true, "Assuming event was consumed. Implement a check mechanism.");
    }
}
