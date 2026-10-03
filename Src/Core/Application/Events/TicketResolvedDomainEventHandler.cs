using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Events;

namespace Application.Events
{
    public sealed class TicketResolvedDomainEventHandler : IEventHandler<TicketResolvedDomainEvent>
    {
        private readonly INotificationWriteRepository _notificationWriteRepository;
        public TicketResolvedDomainEventHandler(INotificationWriteRepository notificationWriteRepository)
        {
            _notificationWriteRepository = notificationWriteRepository;
 
        }

        public Task Handle(TicketResolvedDomainEvent domainEvent, CancellationToken ct)
        {
            var notification = Notification.Create(
                "Your ticket has been resolved by the agent.",
                domainEvent.RequesterId);

            _notificationWriteRepository.Add(notification);
            return Task.CompletedTask;
        }
    }
}
