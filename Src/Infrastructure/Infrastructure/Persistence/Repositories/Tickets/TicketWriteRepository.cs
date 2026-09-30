using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Tickets
{
    public sealed class TicketWriteRepository : ITicketWriteRepository
    {
        private readonly SupportFlowDbContext _context;

        public TicketWriteRepository(SupportFlowDbContext context)
        {
            _context = context;
        }


        public void Add(Ticket ticket)
        {
            _context.Tickets.Add(ticket);
        }

        public async Task<Ticket?> GetTicketByIdAsync(Guid ticketId, CancellationToken ct)
        {
            return await _context.Tickets
                .FirstOrDefaultAsync(t => t.Id == ticketId, ct);
        }

        public void Remove(Ticket ticket)
        {
            _context.Tickets.Remove(ticket);
        }
    }
}
