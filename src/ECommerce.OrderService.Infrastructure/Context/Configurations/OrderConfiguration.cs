using ECommerce.OrderService.Domain.Entities;
using ECommerce.OrderService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.OrderService.Infrastructure.Context.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value,
                value => OrderId.Create(value))
            .ValueGeneratedNever();

        builder.Property(o => o.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasIndex(o => o.OrderNumber)
            .IsUnique();

        builder.Property(o => o.UserId)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(o => o.UserId);

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.ComplexProperty(o => o.ShippingAddress, address =>
        {
            address.Property(a => a.Neighborhood).HasMaxLength(200).IsRequired();
            address.Property(a => a.Street).HasMaxLength(100).IsRequired();
            address.Property(a => a.BuildingNumber).HasMaxLength(100).IsRequired();
            address.Property(a => a.ApartmentNumber).HasMaxLength(20).IsRequired();
            address.Property(a => a.District).HasMaxLength(100).IsRequired();
            address.Property(a => a.City).HasMaxLength(100).IsRequired();
            address.Property(a => a.PostalCode).HasMaxLength(20).IsRequired();
            address.Property(a => a.Country).HasMaxLength(200).IsRequired();
        });

        builder.ComplexProperty(o => o.TotalAmount, money =>
        {
            money.Property(m => m.Amount)
                .HasColumnName("TotalAmount")
                .HasPrecision(18, 2)
                .IsRequired();

            money.Property(m => m.Currency)
                .HasColumnName("Currency")
                .HasMaxLength(3)
                .IsRequired();
        });

        builder.Property(o => o.CreatedAt).IsRequired();
        builder.Property(o => o.UpdatedAt);
        builder.Property(o => o.PaidAt);
        builder.Property(o => o.ShippedAt);
        builder.Property(o => o.DeliveredAt);
        builder.Property(o => o.CancellationReason).HasMaxLength(500);

        builder.HasMany<OrderItem>()
            .WithOne()
            .HasForeignKey("OrderId")
            .OnDelete(DeleteBehavior.Cascade);

        builder.Ignore(o => o.DomainEvents);
    }
}