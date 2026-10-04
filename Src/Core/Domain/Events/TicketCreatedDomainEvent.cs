
namespace Domain.Events
{
    public sealed record TicketCreatedDomainEvent(Guid RequesterId) : IDomainEvent;
}
