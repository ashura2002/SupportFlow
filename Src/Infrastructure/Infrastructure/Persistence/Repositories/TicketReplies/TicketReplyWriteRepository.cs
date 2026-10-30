using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Persistence.Repositories.TicketReplies
{
    public sealed class TicketReplyWriteRepository : ITicketReplyWriteRepository
    {
        private readonly TenantDbContext _context;
        public TicketReplyWriteRepository(TenantDbContext context)
        {
            _context = context;
        }

        public void Add(TicketReply ticketReply)
        {
            _context.TicketReplies.Add(ticketReply);
        }
    }
}
