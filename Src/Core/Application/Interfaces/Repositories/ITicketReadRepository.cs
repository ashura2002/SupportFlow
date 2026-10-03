using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface ITicketReadRepository
    {
        Task<PaginatedResult<TicketResponse>> GetAllTicketsAsync(int page, int pageSize, CancellationToken ct);
        Task<TicketDetailsResponse?> GetMyTicketByIdAsync(Guid ticketId, Guid userId, CancellationToken ct);
        Task<TicketDetailsResponse?> GetTicketByIdAsync(Guid ticketId, CancellationToken ct);
        Task<PaginatedResult<TicketResponse>> GetAllMyTicketsAsync(Guid userId, int page, int pageSize, CancellationToken ct);
        Task<PaginatedResult<TicketResponse>> GetAllMyAssignedTicketsAsync(Guid userId, int page, int pageSize, CancellationToken ct);
    }
}
