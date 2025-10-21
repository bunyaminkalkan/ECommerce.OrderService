using ECommerce.OrderService.Shared.Outbox;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ECommerce.OrderService.Infrastructure.Context.Configurations;

public class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
{
    public void Configure(EntityTypeBuilder<OutboxMessage> builder)
    {
        builder.ToTable("OutboxMessages");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).IsRequired().HasMaxLength(200);
        builder.Property(x => x.Content).IsRequired();
        builder.Property(x => x.OccurredOn).IsRequired();
        builder.HasIndex(x => new { x.ProcessedOn, x.OccurredOn });
    }
}
