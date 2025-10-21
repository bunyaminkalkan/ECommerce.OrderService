using ECommerce.OrderService.Application.Common.Interfaces;
using ECommerce.OrderService.Domain.Events;
using Space.Abstraction;

namespace ECommerce.OrderService.Infrastructure.Services;

public class DomainEventDispatcher(ISpace space) : IDomainEventDispatcher
{
    public async Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        await space.Publish(domainEvent, cancellationToken);
    }
}
