using ECommerce.OrderService.Domain.Entities;
using ECommerce.OrderService.Shared.Outbox;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.OrderService.Application.Context.PostgreSQL;

public interface IAppDbContext
{
    DbSet<Order> Orders { get; }
    DbSet<OrderItem> OrderItems { get; }
    DbSet<OutboxMessage> OutboxMessages { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
