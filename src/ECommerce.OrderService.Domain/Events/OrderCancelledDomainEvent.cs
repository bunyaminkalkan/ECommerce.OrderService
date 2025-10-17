namespace ECommerce.OrderService.Domain.Events;

public record OrderCancelledDomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid OrderId { get; }
    public Guid UserId { get; }
    public string Reason { get; }

    public OrderCancelledDomainEvent(Guid orderId, Guid userId, string reason)
    {
        OrderId = orderId;
        UserId = userId;
        Reason = reason;
    }
}
