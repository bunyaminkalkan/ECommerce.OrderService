namespace ECommerce.OrderService.Shared.Outbox;

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string Type { get; private set; } = default!;
    public string Content { get; private set; } = default!;
    public DateTime OccurredOn { get; private set; }
    public DateTime? ProcessedOn { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; }

    private OutboxMessage() { } // EF Core

    private OutboxMessage(string type, string content)
    {
        Id = Guid.CreateVersion7();
        Type = type;
        Content = content;
        OccurredOn = DateTime.UtcNow;
        RetryCount = 0;
    }

    public static OutboxMessage Create(string type, string content)
        => new(type, content);

    public void MarkAsProcessed()
    {
        ProcessedOn = DateTime.UtcNow;
    }

    public void MarkAsFailed(string error)
    {
        Error = error;
        RetryCount++;
    }
}
