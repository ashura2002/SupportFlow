
using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class TicketAuditEntry : BaseEntity
{
    public Guid TicketId { get; private set; }
    public Guid? ActorUserId { get; private set; }
    public TicketAuditAction Action { get; private set; }
    public TicketStatus? OldValue { get; private set; }
    public TicketStatus? NewValue { get; private set; }
    public Guid TenantId { get; private set; }


    private TicketAuditEntry(
        Guid ticketId,
        Guid? actorUserId,
        TicketAuditAction action,
        TicketStatus? oldValue,
        TicketStatus? newValue,
        Guid tenantId
    )
    {
        TicketId = ticketId;
        ActorUserId = actorUserId;
        Action = action;
        OldValue = oldValue;
        NewValue = newValue;
        TenantId = tenantId;
    }


    public static TicketAuditEntry Create(
        Guid ticketId,
        Guid? actorUserId,
        TicketAuditAction action,
        TicketStatus? oldValue,
        TicketStatus? newValue,
        Guid tenantId
    )
    {
        if (ticketId == Guid.Empty)
            throw new DomainRuleViolationException("Ticket Id cannot be empty.");

        if (!Enum.IsDefined(action))
            throw new DomainRuleViolationException("Invalid ticket audit action.");

        return new TicketAuditEntry(
            ticketId,
            actorUserId,
            action,
            oldValue,
            newValue,
            tenantId
        );
    }
}
