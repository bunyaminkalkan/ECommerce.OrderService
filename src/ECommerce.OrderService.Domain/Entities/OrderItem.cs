namespace ECommerce.OrderService.Domain.Entities;

using ECommerce.OrderService.Domain.Common;
using ECommerce.OrderService.Domain.ValueObjects;

public class OrderItem : BaseEntity<Guid>
{
    public Guid ProductId { get; private set; }
    public string ProductName { get; private set; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Money TotalPrice { get; private set; }

    private OrderItem() { } // EF Core

    private OrderItem(Guid productId, string productName, int quantity, Money unitPrice)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

        Id = Guid.NewGuid();
        ProductId = productId;
        ProductName = productName ?? throw new ArgumentNullException(nameof(productName));
        Quantity = quantity;
        UnitPrice = unitPrice ?? throw new ArgumentNullException(nameof(unitPrice));
        TotalPrice = unitPrice.Multiply(quantity);
        CreatedAt = DateTime.UtcNow;
    }

    public static OrderItem Create(Guid productId, string productName, int quantity, Money unitPrice)
        => new(productId, productName, quantity, unitPrice);

    public void UpdateQuantity(int newQuantity)
    {
        if (newQuantity <= 0)
            throw new ArgumentException("Quantity must be greater than zero", nameof(newQuantity));

        Quantity = newQuantity;
        TotalPrice = UnitPrice.Multiply(newQuantity);
        UpdatedAt = DateTime.UtcNow;
    }
}