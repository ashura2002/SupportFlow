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
            var admin = await _userReadRepository.GetAdminAsync(ct);
            if (admin is null)
                return;

            var notification = Notification.Create(
                "New ticket created.", 
                admin.Id);
            _notificationWriteRepository.Add(notification);


            _logger.LogInformation(
                "New ticket created by {RequesterId}", 
                domainEvent.RequesterId);
        }
    }
}
