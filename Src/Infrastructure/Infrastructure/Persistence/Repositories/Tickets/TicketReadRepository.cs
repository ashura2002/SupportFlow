using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Tickets
{
    public sealed class TicketReadRepository : ITicketReadRepository
    {
        private readonly SupportFlowDbContext _context;
        public TicketReadRepository(SupportFlowDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<TicketResponse>> GetAllMyAssignedTicketsAsync(Guid userId, int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Tickets
                .AsNoTracking()
                .Where(t => t.AssignedAgentId == userId);

            var totalCount = await query.CountAsync(ct);

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                .Join(_context.Categories,
                ticket => ticket.CategoryId,
                category => category.Id,
                (ticket, category) => new
                {
                    ticket,
                    category
                })
                .Join(_context.Users,
                x => x.ticket.RequesterId,
                requester => requester.Id,
                (x, requester) => new
                {
                    x.ticket,
                    x.category,
                    requester
                })
                .GroupJoin(_context.Users,
                x => x.ticket.AssignedAgentId,
                agent => (Guid?)agent.Id,
                (x, agent) => new
                {
                    x.ticket,
                    x.category,
                    x.requester,
                    agent
                })
                .SelectMany(
                x => x.agent.DefaultIfEmpty(),
                (x, agent) => new TicketResponse(
                    x.ticket.Id,
                    x.ticket.TicketNumber,
                    x.ticket.Title,
                    x.ticket.Description,
                    x.ticket.Status,
                    x.ticket.Priority,
                    x.category.Name,
                    x.requester.FullName,
                    agent == null ? null : agent.FullName,
                    x.ticket.DueAt,
                    x.ticket.CreatedAt))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedResult<TicketResponse>(tickets, page, pageSize, totalCount);
        }

        public async Task<PaginatedResult<TicketResponse>> GetAllMyTicketsAsync(Guid userId, int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Tickets
                .AsNoTracking()
                .Where(t => t.RequesterId == userId);

            var totalCount = await query.CountAsync(ct);

            var tickets = await query
                .OrderByDescending(t => t.CreatedAt)
                // ticket to category
                .Join(_context.Categories,
                ticket => ticket.CategoryId,
                category => category.Id,
                (ticket, category) => new
                {
                    ticket,
                    category
                })
                // ticket to requester
                .Join(_context.Users,
                x => x.ticket.RequesterId,
                requester => requester.Id,
                (x, requester) => new
                {
                    x.ticket,
                    x.category,
                    requester
                })
                // ticket to assigned agent
                .GroupJoin(_context.Users,
                x => x.ticket.AssignedAgentId,
                agent => (Guid?)agent.Id,
                (x, agents) => new
                {
                    x.ticket,
                    x.category,
                    x.requester,
                    agents
                })
                .SelectMany(
                x => x.agents.DefaultIfEmpty(),
                (x, agent) => new TicketResponse(
                    x.ticket.Id,
                    x.ticket.TicketNumber,
                    x.ticket.Title,
                    x.ticket.Description,
                    x.ticket.Status,
                    x.ticket.Priority,
                    x.category.Name,
                    x.requester.FullName,
                    agent == null ? null : agent.FullName,
                    x.ticket.DueAt,
                    x.ticket.CreatedAt))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedResult<TicketResponse>(tickets, page, pageSize, totalCount);
        }

        public async Task<PaginatedResult<TicketResponse>> GetAllTicketsAsync(int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Tickets.AsNoTracking();

            var totalCount = await query.CountAsync(ct);

            var tickets = await query.OrderByDescending(t => t.CreatedAt)
                // ticket to category
                .Join(_context.Categories,
                ticket => ticket.CategoryId,
                category => category.Id,
                (ticket, category) => new
                {
                    ticket,
                    category
                })
                // ticket to requester
                .Join(_context.Users,
                x => x.ticket.RequesterId,
                requester => requester.Id,
                (x, requester) => new
                {
                    x.ticket,
                    x.category,
                    requester
                })
                // ticket to assigned agent
                .GroupJoin(_context.Users,
                x => x.ticket.AssignedAgentId,
                agent => (Guid?)agent.Id,
                (x, agents) => new
                {
                    x.ticket,
                    x.category,
                    x.requester,
                    agents
                })
                .SelectMany(
                x => x.agents.DefaultIfEmpty(),
                (x, agent) => new TicketResponse(
                    x.ticket.Id,
                    x.ticket.TicketNumber,
                    x.ticket.Title,
                    x.ticket.Description,
                    x.ticket.Status,
                    x.ticket.Priority,
                    x.category.Name,
                    x.requester.FullName,
                    agent == null ? null : agent.FullName,
                    x.ticket.DueAt,
                    x.ticket.CreatedAt))
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PaginatedResult<TicketResponse>(tickets, page, pageSize, totalCount);
        }

        public async Task<TicketDetailsResponse?> GetMyTicketByIdAsync(Guid ticketId, Guid userId, CancellationToken ct)
        {
            var query = _context.Tickets.AsNoTracking();
            var ticket = await query
                    .Where(t => t.Id == ticketId && t.RequesterId == userId)
                    .Join(_context.Categories,
                    ticket => ticket.CategoryId,
                    category => category.Id,
                    (ticket, category) => new
                    {
                        ticket,
                        category
                    })
                    .Join(_context.Users,
                    x => x.ticket.RequesterId,
                    requester => requester.Id,
                    (x, requester) => new
                    {
                        x.ticket,
                        x.category,
                        requester,
                    })
                     .GroupJoin(_context.Users,
                   x => x.ticket.AssignedAgentId,
                   agent => (Guid?)agent.Id,
                   (x, agent) => new
                   {
                       x.ticket,
                       x.category,
                       x.requester,
                       agent
                   })
                   .SelectMany(
                   x => x.agent.DefaultIfEmpty(),
                   (x, agent) => new TicketResponse(
                       x.ticket.Id,
                       x.ticket.TicketNumber,
                       x.ticket.Title,
                       x.ticket.Description,
                       x.ticket.Status,
                       x.ticket.Priority,
                       x.category.Name,
                       x.requester.FullName,
                       agent == null ? null : agent.FullName,
                       x.ticket.DueAt,
                       x.ticket.CreatedAt))
                   .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;

            var ticketReplies = await _context.TicketReplies
                 .Where(t => t.TicketId == ticket.Id)
                 .Join(_context.Users,
                 ticketReply => ticketReply.AuthorId,
                 author => author.Id,
                 (ticketReply, author) => new
                 {
                     ticketReply,
                     author
                 })
                 .OrderBy(t => t.ticketReply.CreatedAt)
                 .Select(t => new TicketReplyResponse(
                     t.ticketReply.Id,
                     t.author.Id,
                     t.author.FullName,
                     t.ticketReply.Content,
                     t.ticketReply.CreatedAt))
                 .ToListAsync(ct);

            return new TicketDetailsResponse(ticket, ticketReplies);

        }

        public async Task<TicketDetailsResponse?> GetTicketByIdAsync(Guid ticketId, CancellationToken ct)
        {
            var query = _context.Tickets.AsNoTracking();
            var ticket = await query
                .Where(t => t.Id == ticketId)
                .Join(_context.Categories,
                ticket => ticket.CategoryId,
                category => category.Id,
                (ticket, category) => new
                {
                    ticket,
                    category
                })
                .Join(_context.Users,
                x => x.ticket.RequesterId,
                requester => requester.Id,
                (x, requester) => new
                {
                    x.ticket,
                    x.category,
                    requester
                })
                .GroupJoin(_context.Users,
                x => x.ticket.AssignedAgentId,
                agent => (Guid?)agent.Id,
                (x, agent) => new
                {
                    x.ticket,
                    x.category,
                    x.requester,
                    agent
                })
                 .SelectMany(
                   x => x.agent.DefaultIfEmpty(),
                   (x, agent) => new TicketResponse(
                       x.ticket.Id,
                       x.ticket.TicketNumber,
                       x.ticket.Title,
                       x.ticket.Description,
                       x.ticket.Status,
                       x.ticket.Priority,
                       x.category.Name,
                       x.requester.FullName,
                       agent == null ? null : agent.FullName,
                       x.ticket.DueAt,
                       x.ticket.CreatedAt))
                   .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;

            var ticketReplies = await _context.TicketReplies
              .Where(t => t.TicketId == ticket.Id)
              .Join(_context.Users,
              ticketReply => ticketReply.AuthorId,
              author => author.Id,
              (ticketReply, author) => new
              {
                  ticketReply,
                  author
              })
              .OrderBy(t => t.ticketReply.CreatedAt)
              .Select(t => new TicketReplyResponse(
                  t.ticketReply.Id,
                  t.author.Id,
                  t.author.FullName,
                  t.ticketReply.Content,
                  t.ticketReply.CreatedAt))
              .ToListAsync(ct);

            return new TicketDetailsResponse(ticket, ticketReplies);
        }
    }
}
