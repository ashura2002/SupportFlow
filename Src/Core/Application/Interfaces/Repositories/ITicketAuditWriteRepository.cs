using Domain.Entities;

namespace Application.Interfaces.Repositories;

public interface ITicketAuditWriteRepository
{
    void Add(TicketAuditEntry ticketAuditEntry);
}
