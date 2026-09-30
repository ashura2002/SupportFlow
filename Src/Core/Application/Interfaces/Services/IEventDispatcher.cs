
using Domain.Events;

namespace Application.Interfaces.Services
{
    public interface IEventDispatcher
    {
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents,  CancellationToken ct);
    }
}
