using ECommerce.OrderService.Application.Common.Interfaces;
using ECommerce.OrderService.Application.Context.PostgreSQL;
using ECommerce.OrderService.Domain.Common;
using ECommerce.OrderService.Domain.Entities;
using ECommerce.OrderService.Domain.ValueObjects;
using ECommerce.OrderService.Shared.Outbox;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json;

namespace ECommerce.OrderService.Infrastructure.Context.PostgreSQL;

public class AppDbContext : DbContext, IAppDbContext
{
    private readonly IDomainEventDispatcher _domainEventDispatcher;
    public AppDbContext(DbContextOptions<AppDbContext> options, IDomainEventDispatcher domainEventDispatcher) : base(options)
    {
        _domainEventDispatcher = domainEventDispatcher;
    }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderItem> OrderItems { get; set; }

    public DbSet<OutboxMessage> OutboxMessages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // Domain Events'leri topla
        var domainEntities = ChangeTracker
            .Entries<BaseEntity<OrderId>>()
            .Where(x => x.Entity.DomainEvents.Any())
            .Select(x => x.Entity)
            .ToList();

        var domainEvents = domainEntities
            .SelectMany(x => x.DomainEvents)
            .ToList();

        // Domain Events'leri temizle
        domainEntities.ForEach(entity => entity.ClearDomainEvents());

        // Outbox pattern için event'leri kaydet
        foreach (var domainEvent in domainEvents)
        {
            var outboxMessage = OutboxMessage.Create(
                domainEvent.GetType().Name,
                JsonSerializer.Serialize(domainEvent, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                }));

            OutboxMessages.Add(outboxMessage);
        }

        var result = await base.SaveChangesAsync(cancellationToken);

        // Space ile Domain Events'leri dispatch et
        foreach (var domainEvent in domainEvents)
        {
            //_logger.LogInformation("Dispatching domain event: {EventType}", domainEvent.GetType().Name);
            await _domainEventDispatcher.DispatchAsync(domainEvent, cancellationToken);
        }

        return result;
    }
}
