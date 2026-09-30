namespace Domain.Events
{
    public sealed record SupportAgentAssignedDomainEvent(Guid AgentId, Guid TicketId) : IDomainEvent;
}
