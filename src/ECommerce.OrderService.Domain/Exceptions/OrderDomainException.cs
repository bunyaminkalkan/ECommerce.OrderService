namespace ECommerce.OrderService.Domain.Exceptions;

public class OrderDomainException : Exception
{
    public OrderDomainException(string message) : base(message) { }

    public OrderDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
