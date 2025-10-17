namespace ECommerce.OrderService.Domain.Events;

public record OrderPaidDomainEvent : IDomainEvent
{
    public Guid EventId { get; } = Guid.NewGuid();
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
    public Guid OrderId { get; }
    public decimal Amount { get; }

    public OrderPaidDomainEvent(Guid orderId, decimal amount)
    {
        OrderId = orderId;
        Amount = amount;
    }
}
