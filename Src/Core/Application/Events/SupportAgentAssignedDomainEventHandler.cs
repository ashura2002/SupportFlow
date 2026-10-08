using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Logging;


namespace Application.Events
{
    public sealed class SupportAgentAssignedDomainEventHandler : IEventHandler<SupportAgentAssignedDomainEvent>
    {
        private readonly INotificationWriteRepository _notificationWriteRepository;
        private readonly ILogger<SupportAgentAssignedDomainEventHandler> _logger;

        public SupportAgentAssignedDomainEventHandler(
            INotificationWriteRepository notificationWriteRepository,
            ILogger<SupportAgentAssignedDomainEventHandler> logger)
        {
            _notificationWriteRepository = notificationWriteRepository;
            _logger = logger;
        }

        public Task Handle(SupportAgentAssignedDomainEvent domainEvent, CancellationToken ct)
        {
            var notification = Notification.Create($"You have been assigned to a new ticket {domainEvent.TicketId}", domainEvent.AgentId);

            _notificationWriteRepository.Add(notification);

            _logger.LogInformation(
            "Notification created for agent {AgentId} on ticket {TicketId}",
            domainEvent.AgentId,
            domainEvent.TicketId);

            return Task.CompletedTask;
        }
    }
}
