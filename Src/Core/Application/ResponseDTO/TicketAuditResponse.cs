using Domain.Enums;

namespace Application.ResponseDTO;

public sealed record TicketAuditResponse(
    Guid TicketAuditId,
    Guid? ActorUserId,
    TicketAuditAction Action,
    TicketStatus? OldValue,
    TicketStatus? NewValue,
    DateTime CreatedAt
);