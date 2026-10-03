using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;

namespace Infrastructure.Persistence.Repositories.TicketReplies
{
    public sealed class TicketReplyWriteRepository : ITicketReplyWriteRepository
    {
        private readonly SupportFlowDbContext _context;
        public TicketReplyWriteRepository(SupportFlowDbContext context)
        {
            _context = context;
        }

        public void Add(TicketReply ticketReply)
        {
            _context.TicketReplies.Add(ticketReply);
        }
    }
}
