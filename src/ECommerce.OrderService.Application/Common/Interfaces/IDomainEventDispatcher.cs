using ECommerce.OrderService.Domain.Events;

namespace ECommerce.OrderService.Application.Common.Interfaces;

public interface IDomainEventDispatcher
{
    Task DispatchAsync(IDomainEvent domainEvent, CancellationToken cancellationToken = default);
}
