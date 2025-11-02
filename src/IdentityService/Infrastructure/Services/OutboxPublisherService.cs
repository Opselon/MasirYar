// src/IdentityService/Infrastructure/Services/OutboxPublisherService.cs
using System.Text;
using Core.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Infrastructure.Services;

public interface IOutboxPublisherService
{
    Task PublishPendingMessagesAsync(CancellationToken cancellationToken);
}

public class OutboxPublisherService : IOutboxPublisherService
{
    private readonly IdentityDbContext _dbContext;
    private readonly ILogger<OutboxPublisherService> _logger;
    private readonly IConnection _rabbitConnection;

    public OutboxPublisherService(
        IdentityDbContext dbContext,
        ILogger<OutboxPublisherService> logger,
        IConfiguration configuration) // Inject IConfiguration
    {
        _dbContext = dbContext;
        _logger = logger;

        var rabbitMqHost = configuration["RabbitMq:HostName"] ?? "rabbitmq";
        var factory = new ConnectionFactory() { HostName = rabbitMqHost };
        _rabbitConnection = factory.CreateConnection();
    }

    public async Task PublishPendingMessagesAsync(CancellationToken cancellationToken)
    {
        var pendingMessages = await _dbContext.OutboxMessages
            .Where(m => m.ProcessedOnUtc == null)
            .OrderBy(m => m.OccurredOnUtc)
            .Take(20) // Process in batches
            .ToListAsync(cancellationToken);

        if (!pendingMessages.Any())
        {
            return;
        }

        _logger.LogInformation("Found {Count} pending outbox messages to publish.", pendingMessages.Count);

        using var channel = _rabbitConnection.CreateModel();

        // Ensure the exchange exists
        channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Fanout);

        foreach (var message in pendingMessages)
        {
            try
            {
                var body = Encoding.UTF8.GetBytes(message.Content);
                channel.BasicPublish(
                    exchange: "user_events",
                    routingKey: "", // Not used in fanout exchanges
                    basicProperties: null,
                    body: body);

                message.ProcessedOnUtc = DateTime.UtcNow;
                _logger.LogInformation("Published message {MessageId} of type {MessageType}.", message.Id, message.Type);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to publish message {MessageId}.", message.Id);
                message.Error = ex.Message;
            }
        }

        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
