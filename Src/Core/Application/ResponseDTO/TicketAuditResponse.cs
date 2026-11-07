using Domain.Enums;

namespace Application.ResponseDTO;

public sealed record TicketAuditResponse(
    Guid Id,
    Guid? ActorUserId,
    TicketAuditAction Action,
    TicketStatus? OldValue,
    TicketStatus? NewValue,
    DateTime CreatedAt
);