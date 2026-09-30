using Domain.Events;

namespace Application.Interfaces.Services
{
    public interface IEventHandler<TEvent> where TEvent:IDomainEvent
    {
        Task Handle(TEvent domainEvent, CancellationToken ct);
    }
}
