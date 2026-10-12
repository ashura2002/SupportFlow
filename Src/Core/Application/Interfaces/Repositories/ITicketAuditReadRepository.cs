

using Application.ResponseDTO;

namespace Application.Interfaces.Repositories;

public interface ITicketAuditReadRepository
{
    Task<IReadOnlyList<TicketAuditResponse>> GetTicketAuditByIdAsync(Guid  ticketId, CancellationToken ct);
}
