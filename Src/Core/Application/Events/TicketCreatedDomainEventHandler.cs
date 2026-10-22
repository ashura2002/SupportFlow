using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Events;
using Microsoft.Extensions.Logging;

namespace Application.Events
{
    public sealed class TicketCreatedDomainEventHandler : IEventHandler<TicketCreatedDomainEvent>
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly INotificationWriteRepository _notificationWriteRepository;
        private readonly ILogger<TicketCreatedDomainEventHandler> _logger;
        public TicketCreatedDomainEventHandler(
            IUserReadRepository userReadRepository,
            INotificationWriteRepository notificationWriteRepository,
            ILogger<TicketCreatedDomainEventHandler> logger)
        {
            _userReadRepository = userReadRepository;
            _notificationWriteRepository = notificationWriteRepository;
            _logger = logger;
        }

        public async Task Handle(TicketCreatedDomainEvent domainEvent, CancellationToken ct)
        {

            var admins = await _userReadRepository.GetTenantAdminsAsync(domainEvent.TenantId, ct);

            if (admins.Count == 0)
            {
                _logger.LogWarning(
                    "No active TenantAdmins found for tenant {TenantId}",
                    domainEvent.TenantId);
                return;
            }


            foreach (var admin in admins)
            {
                var notification = Notification.Create(
               "New ticket created.",
               admin.Id);
                _notificationWriteRepository.Add(notification);
            }

            _logger.LogInformation(
                "Created ticket notifications for {AdminCount} TenantAdmins. RequesterId: {RequesterId}, TenantId: {TenantId}",
                admins.Count,
                domainEvent.RequesterId,
                domainEvent.TenantId);
        }
    }
}
