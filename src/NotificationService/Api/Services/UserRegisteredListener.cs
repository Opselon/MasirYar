// src/NotificationService/Api/Services/UserRegisteredListener.cs
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace Api.Services;

public class UserRegisteredListener : IHostedService
{
    private readonly ILogger<UserRegisteredListener> _logger;
    private readonly IConnection _connection;
    private readonly IModel _channel;

    public UserRegisteredListener(ILogger<UserRegisteredListener> logger, IConfiguration configuration)
    {
        _logger = logger;
        var rabbitMqHost = configuration["RabbitMq:HostName"] ?? "rabbitmq";
        var factory = new ConnectionFactory() { HostName = rabbitMqHost, DispatchConsumersAsync = true };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting User Registered Listener...");

        _channel.ExchangeDeclare(exchange: "user_events", type: ExchangeType.Fanout);

        var queueName = _channel.QueueDeclare(durable: true).QueueName;
        _channel.QueueBind(queue: queueName,
                           exchange: "user_events",
                           routingKey: "");

        var consumer = new AsyncEventingBasicConsumer(_channel);
        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            try
            {
                var userEvent = JsonSerializer.Deserialize<UserRegisteredEvent>(message);
                _logger.LogInformation(
                    "Received UserRegisteredEvent for user {Username} with email {Email}. Sending welcome email...",
                    userEvent?.Username, userEvent?.Email);

                // Simulate sending email
                await Task.Delay(1000, cancellationToken);

                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "Failed to deserialize message: {Message}", message);
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: false);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Failed to process message: {Message}", message);
                _channel.BasicNack(deliveryTag: ea.DeliveryTag, multiple: false, requeue: true); // Requeue for transient errors
            }
        };

        _channel.BasicConsume(queue: queueName,
                             autoAck: false, // Manual acknowledgement
                             consumer: consumer);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping User Registered Listener...");
        _channel.Close();
        _connection.Close();
        return Task.CompletedTask;
    }
}

// A simple DTO to deserialize the event content
public class UserRegisteredEvent
{
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
}
