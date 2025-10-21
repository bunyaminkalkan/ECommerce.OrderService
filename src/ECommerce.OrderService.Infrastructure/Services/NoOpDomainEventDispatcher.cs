using ECommerce.OrderService.Application.Common.Interfaces;
using ECommerce.OrderService.Domain.Events;

namespace ECommerce.OrderService.Infrastructure.Services;

public class NoOpDomainEventDispatcher : IDomainEventDispatcher
{
    public Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
