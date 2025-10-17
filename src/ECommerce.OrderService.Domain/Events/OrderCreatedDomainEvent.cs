namespace ECommerce.OrderService.Domain.Events;

public record OrderCreatedDomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid OrderId { get; }
    public Guid UserId { get; }
    public decimal TotalAmount { get; }

    public OrderCreatedDomainEvent(Guid orderId, Guid userId, decimal totalAmount)
    {
        OrderId = orderId;
        UserId = userId;
        TotalAmount = totalAmount;
    }
}