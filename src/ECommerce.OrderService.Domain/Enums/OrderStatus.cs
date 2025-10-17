namespace ECommerce.OrderService.Domain.Enums;

public enum OrderStatus
{
    Pending = 1,
    AwaitingPayment = 2,
    PaymentConfirmed = 3,
    StockConfirmed = 4,
    Shipped = 5,
    Delivered = 6,
    Cancelled = 7,
    Failed = 8
}