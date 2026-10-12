using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.TicketAuditEntry;

public sealed class TicketAuditReadRepository : ITicketAuditReadRepository
{
    private readonly SupportFlowDbContext _context;
    public TicketAuditReadRepository(SupportFlowDbContext context)
    {
        _context = context;
    }
    public async Task<IReadOnlyList<TicketAuditResponse>> GetTicketAuditByIdAsync(Guid ticketId, CancellationToken ct)
    {
        return await _context.Database
            .SqlQuery<TicketAuditResponse>(
                $"""
                    SELECT
                        ta."Id" AS "TicketAuditId",
                        ta."ActorUserId",
                        ta."Action",
                        ta."OldValue",
                        ta."NewValue",
                        ta."CreatedAt"
                    FROM "TicketAuditEntries" AS ta
                    WHERE ta."TicketId" = {ticketId}
                    ORDER BY ta."CreatedAt" DESC
                """
            )
            .ToListAsync(ct);
    }
}
