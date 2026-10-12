using Application.Interfaces.Repositories;
using Infrastructure.Data;

namespace Infrastructure.Persistence.Repositories.TicketAuditEntry;

public sealed class TicketAuditWriteRepository : ITicketAuditWriteRepository
{
    private readonly SupportFlowDbContext _context;
    public TicketAuditWriteRepository(SupportFlowDbContext context)
    {
        _context = context;
    }


    public void Add(Domain.Entities.TicketAuditEntry ticketAuditEntry)
    {
        _context.TicketAuditEntries.Add(ticketAuditEntry);
    }
}
