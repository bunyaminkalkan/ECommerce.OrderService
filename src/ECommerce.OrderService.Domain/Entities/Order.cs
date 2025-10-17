namespace ECommerce.OrderService.Domain.Entities;

using ECommerce.OrderService.Domain.Common;
using ECommerce.OrderService.Domain.Enums;
using ECommerce.OrderService.Domain.Events;
using ECommerce.OrderService.Domain.Exceptions;
using ECommerce.OrderService.Domain.ValueObjects;

public class Order : BaseEntity<OrderId>, IAggregateRoot
{
    private readonly List<OrderItem> _items = new();

    public Guid UserId { get; private set; }
    public string OrderNumber { get; private set; }
    public OrderStatus Status { get; private set; }
    public Address ShippingAddress { get; private set; }
    public Money TotalAmount { get; private set; }
    public DateTime? PaidAt { get; private set; }
    public DateTime? ShippedAt { get; private set; }
    public DateTime? DeliveredAt { get; private set; }
    public string? CancellationReason { get; private set; }

    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();

    private Order() { } // EF Core

    private Order(Guid userId, Address shippingAddress, List<OrderItem> items)
    {
        Id = OrderId.Create();
        UserId = userId;
        OrderNumber = GenerateOrderNumber();
        Status = OrderStatus.Pending;
        ShippingAddress = shippingAddress ?? throw new ArgumentNullException(nameof(shippingAddress));
        CreatedAt = DateTime.UtcNow;

        if (items == null || !items.Any())
            throw new OrderDomainException("Order must have at least one item");

        _items.AddRange(items);
        CalculateTotalAmount();

        AddDomainEvent(new OrderCreatedDomainEvent(Id.Value, UserId, TotalAmount.Amount));
    }

    public static Order Create(Guid userId, Address shippingAddress, List<OrderItem> items)
        => new(userId, shippingAddress, items);

    public void ConfirmPayment()
    {
        if (Status != OrderStatus.AwaitingPayment && Status != OrderStatus.Pending)
            throw new OrderDomainException($"Cannot confirm payment for order in {Status} status");

        Status = OrderStatus.PaymentConfirmed;
        PaidAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderPaidDomainEvent(Id.Value, TotalAmount.Amount));
    }

    public void ConfirmStock()
    {
        if (Status != OrderStatus.PaymentConfirmed)
            throw new OrderDomainException($"Cannot confirm stock for order in {Status} status");

        Status = OrderStatus.StockConfirmed;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsShipped()
    {
        if (Status != OrderStatus.StockConfirmed)
            throw new OrderDomainException($"Cannot ship order in {Status} status");

        Status = OrderStatus.Shipped;
        ShippedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderShippedDomainEvent(Id.Value, UserId));
    }

    public void MarkAsDelivered()
    {
        if (Status != OrderStatus.Shipped)
            throw new OrderDomainException($"Cannot deliver order in {Status} status");

        Status = OrderStatus.Delivered;
        DeliveredAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason)
    {
        if (Status == OrderStatus.Delivered || Status == OrderStatus.Cancelled)
            throw new OrderDomainException($"Cannot cancel order in {Status} status");

        Status = OrderStatus.Cancelled;
        CancellationReason = reason ?? "No reason provided";
        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new OrderCancelledDomainEvent(Id.Value, UserId, reason));
    }

    public void MarkAsFailed(string reason)
    {
        Status = OrderStatus.Failed;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    private void CalculateTotalAmount()
    {
        var total = _items.Aggregate(
            Money.Create(0),
            (sum, item) => sum.Add(item.TotalPrice)
        );
        TotalAmount = total;
    }

    private static string GenerateOrderNumber()
    {
        var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var random = new Random().Next(1000, 9999);
        return $"ORD-{timestamp}-{random}";
    }
}