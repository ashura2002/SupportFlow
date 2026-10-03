namespace Domain.Events
{
    public sealed record TicketResolvedDomainEvent(Guid RequesterId) : IDomainEvent;
}
