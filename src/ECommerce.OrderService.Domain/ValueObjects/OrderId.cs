namespace ECommerce.OrderService.Domain.ValueObjects;

public record OrderId
{
    public Guid Value { get; }

    private OrderId(Guid value) => Value = value;

    public static OrderId Create() => new(Guid.CreateVersion7());

    public override string ToString() => Value.ToString();
}
