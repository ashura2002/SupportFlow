
namespace Domain.Events
{
    public sealed record TicketCreatedDomainEvent(
        Guid RequesterId,
        Guid TenantId) : IDomainEvent;
}
