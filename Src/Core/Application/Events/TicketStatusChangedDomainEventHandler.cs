using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.Events;

namespace Application.Events;

public sealed class TicketStatusChangedDomainEventHandler : IEventHandler<TicketStatusChangedDomainEvent>
{
    private readonly ITicketAuditWriteRepository _ticketAuditWriteRepository;
    private readonly ICurrentUserService _currentUserService;
    public TicketStatusChangedDomainEventHandler(
        ITicketAuditWriteRepository ticketAuditWriteRepository,
        ICurrentUserService currentUserService
    )
    {
        _ticketAuditWriteRepository = ticketAuditWriteRepository;
        _currentUserService = currentUserService;
    }


    public Task Handle(TicketStatusChangedDomainEvent domainEvent, CancellationToken ct)
    {
        var audit = TicketAuditEntry.Create(
            domainEvent.TicketId,
            _currentUserService.UserId,
            TicketAuditAction.StatusChanged,
            domainEvent.OldStatus,
            domainEvent.NewStatus,
            _currentUserService.TenantId);

        _ticketAuditWriteRepository.Add(audit);
        return Task.CompletedTask;
    }
}
