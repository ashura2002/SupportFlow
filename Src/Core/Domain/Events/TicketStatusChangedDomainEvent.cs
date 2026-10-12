using Domain.Enums;

namespace Domain.Events;

public sealed record TicketStatusChangedDomainEvent(
    Guid TicketId,
    TicketStatus OldStatus,
    TicketStatus NewStatus) : IDomainEvent;
