// src/IdentityService/Core/Entities/OutboxMessage.cs
namespace Core.Entities;

public class OutboxMessage
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Type { get; private set; }
    public string Content { get; private set; }
    public DateTime OccurredOnUtc { get; private set; } = DateTime.UtcNow;
    public DateTime? ProcessedOnUtc { get; set; }
    public string? Error { get; set; }

    // Private constructor for EF Core
    private OutboxMessage() { }

    public OutboxMessage(string type, string content)
    {
        Type = type;
        Content = content;
    }
}
