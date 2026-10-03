using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ITicketReplyWriteRepository
    {
        void Add(TicketReply ticketReply);
    }
}
