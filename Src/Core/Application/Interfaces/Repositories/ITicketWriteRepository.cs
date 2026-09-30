using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ITicketWriteRepository
    {
        void Add(Ticket ticket);
        void Remove(Ticket ticket);
        Task<Ticket?> GetTicketByIdAsync(Guid ticketId, CancellationToken ct);
    }
}
