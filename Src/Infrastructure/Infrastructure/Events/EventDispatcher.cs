using Application.Interfaces.Services;
using Domain.Events;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Events
{
    public sealed class EventDispatcher : IEventDispatcher
    {
        private readonly IServiceProvider _serviceProvider;

        public EventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken ct)
        {
            foreach (var domainEvent in domainEvents)
            {
                // Get the actual event type at runtime
                var handlerType = typeof(IEventHandler<>).MakeGenericType(domainEvent.GetType());


                // Get all handlers registered for this event in DI container
                foreach (var handler in _serviceProvider.GetServices(handlerType))
                {
                    var method = handlerType.GetMethod(nameof(IEventHandler<>.Handle));

                    // Call Handle and pass the event and cancellation token
                    await (Task)method!.Invoke(
                        handler, 
                        new object[] { domainEvent, ct })!;
                }
            }
        }
    }
}
