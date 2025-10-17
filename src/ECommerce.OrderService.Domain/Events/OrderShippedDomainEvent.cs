namespace ECommerce.OrderService.Domain.Events;

public record OrderShippedDomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid OrderId { get; }
    public Guid UserId { get; }

    public OrderShippedDomainEvent(Guid orderId, Guid userId)
    {
        OrderId = orderId;
        UserId = userId;
    }
}
