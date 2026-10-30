using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.TicketAuditEntry;

public sealed class TicketAuditReadRepository : ITicketAuditReadRepository
{
    private readonly TenantDbContext _context;
    public TicketAuditReadRepository(TenantDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<TicketAuditResponse>> GetTicketAuditByIdAsync(
        Guid ticketId, 
        Guid tenantId,
        CancellationToken ct)
    {
        return await _context.TicketAuditEntries
           .AsNoTracking()
           .Where(ta => ta.Id == ticketId)
           .Select(ta => new TicketAuditResponse(
               ta.Id,
               ta.ActorUserId, 
               ta.Action, 
               ta.OldValue, 
               ta.NewValue,
               ta.CreatedAt))
           .ToListAsync(ct);
    }
}
