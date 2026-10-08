using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Domain.Entities;
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

        // supportagent
        public async Task<PaginatedResult<TicketResponse>> GetAllMyAssignedTicketsAsync(
         Guid userId,
         int page,
         int pageSize,
         CancellationToken ct)
        {
            var tickets = await _context.Database
                .SqlQuery<TicketResponse>(
                $"""
                    SELECT 
                        t."Id",
                        t."TicketNumber",
                        t."Title",
                        t."Description",
                        t."Status",
                        t."Priority",
                        c."Name" AS "CategoryName",
                        CONCAT(r."FirstName", ' ', r."LastName") AS "Requester",
                        CASE 
                            WHEN a."Id" IS NULL THEN NULL
                            ELSE CONCAT(a."FirstName", ' ', a."LastName")
                            END AS "AssignedAgent",
                        t."DueAt",
                        t."CreatedAt"
                        FROM "Tickets" AS t

                        INNER JOIN "Categories" AS c
                        ON t."CategoryId" = c."Id"

                        INNER JOIN "Users" AS r
                        ON t."RequesterId" = r."Id"

                        LEFT JOIN "Users" AS a
                        ON t."AssignedAgentId" = a."Id"

                        WHERE t."AssignedAgentId" = {userId}
                        AND t."DeletedAt" IS NULL
                        ORDER BY t."CreatedAt" DESC
                        OFFSET {(page - 1) * pageSize}
                        LIMIT {pageSize}
                """)
                .ToListAsync(ct);

            var totalCount = await _context.Database
                .SqlQuery<int>(
                $"""
                    SELECT COUNT (*) AS "Value"
                    FROM "Tickets"
                    WHERE "AssignedAgentId" = {userId} AND
                    "DeletedAt" IS NULL
                """)
                .SingleAsync(ct);

            return new PaginatedResult<TicketResponse>(
                tickets,
                page,
                pageSize, 
                totalCount);
        }


        // requester
        public async Task<PaginatedResult<TicketResponse>> GetAllMyTicketsAsync(Guid userId, int page, int pageSize, CancellationToken ct)
        {
            var totalCount = await _context.Database
                .SqlQuery<int>(
                $"""
                    SELECT COUNT (*) AS "Value"
                    FROM "Tickets"
                    WHERE "RequesterId" = {userId}
                    AND "DeletedAt" IS NULL
                """)
                .SingleAsync(ct);

            var tickets = await _context.Database
                .SqlQuery<TicketResponse>(
                $"""
                    SELECT 
                        t."Id",
                        t."TicketNumber",
                        t."Title",
                        t."Description",
                        t."Status",
                        t."Priority",
                        c."Name" AS "CategoryName",
                        CONCAT (r."FirstName", ' ' , r."LastName") AS "Requester",
                        CASE 
                            WHEN a."Id" IS NULL THEN NULL
                            ELSE CONCAT(a."FirstName", ' ', a."LastName")
                            END AS "AssignedAgent",
                        t."DueAt",
                        t."CreatedAt"
                    FROM "Tickets" AS t

                    INNER JOIN "Categories" AS c
                    ON t."CategoryId" = c."Id"

                    INNER JOIN "Users" AS r
                    ON t."RequesterId" = r."Id"

                    LEFT JOIN "Users" AS a
                    ON t."AssignedAgentId" = a."Id"

                    WHERE t."RequesterId" = {userId} AND
                    t."DeletedAt" IS NULL
                    ORDER BY t."CreatedAt" DESC
                    OFFSET {(page - 1) * pageSize}
                    LIMIT {pageSize}
                """)
                .ToListAsync(ct);

            return new PaginatedResult<TicketResponse>(tickets, page, pageSize, totalCount);
        }   

        public async Task<PaginatedResult<TicketResponse>> GetAllTicketsAsync(int page, int pageSize, CancellationToken ct)
        {
            var totalCount = await _context.Database
                .SqlQuery<int>(
                $"""
                    SELECT COUNT (*) AS "Value"
                    FROM "Tickets"
                    WHERE "DeletedAt" IS NULL
                """)
                .SingleAsync(ct);

            var tickets = await _context.Database
                .SqlQuery<TicketResponse>(
                $"""
                    SELECT 
                        t."Id",
                        t."TicketNumber",
                        t."Title",
                        t."Description",
                        t."Status",
                        t."Priority",
                        c."Name" AS "CategoryName",
                        CONCAT(r."FirstName", ' ', r."LastName") AS "Requester",
                        CASE 
                            WHEN a."Id" IS NULL THEN NULL
                            ELSE CONCAT(a."FirstName", ' ', a."LastName") 
                            END AS "AssignedAgent",
                        t."DueAt",
                        t."CreatedAt"
                    FROM "Tickets" AS t

                    INNER JOIN "Categories" AS c
                    ON t."CategoryId" = c."Id"

                    INNER JOIN "Users" AS r
                    ON t."RequesterId" = r."Id"

                    LEFT JOIN "Users" AS a
                    ON t."AssignedAgentId" = a."Id"

                    WHERE t."DeletedAt" IS NULL
                    ORDER BY t."CreatedAt" DESC
                    OFFSET {(page - 1) * pageSize}
                    LIMIT {pageSize}
                """)
                .ToListAsync(ct);

            return new PaginatedResult<TicketResponse>(tickets, page, pageSize, totalCount);
        }


        // requester owner the ticket
        public async Task<TicketDetailsResponse?> GetMyTicketByIdAsync(Guid ticketId, Guid userId, CancellationToken ct)
        {
            var ticket = await _context.Database
                .SqlQuery<TicketResponse>(
                $"""
                    SELECT 
                        t."Id",
                        t."TicketNumber",
                        t."Title",
                        t."Description",
                        t."Status",
                        t."Priority",
                        c."Name" AS "CategoryName",
                        CONCAT(r."FirstName", ' ', r."LastName") AS "Requester",
                        CASE 
                            WHEN a."Id" IS NULL THEN NULL
                            ELSE CONCAT(a."FirstName", ' ', a."LastName")
                            END AS "AssignedAgent",
                        t."DueAt",
                        t."CreatedAt"
                    FROM "Tickets" AS t

                    INNER JOIN "Categories" AS c
                    ON t."CategoryId" = c."Id"

                    INNER JOIN "Users" As r
                    ON t."RequesterId" = r."Id"

                    LEFT JOIN "Users" AS a
                    ON t."AssignedAgentId" = a."Id"
                    
                    WHERE t."Id" = {ticketId} 
                    AND t."RequesterId" = {userId}
                    AND t."DeletedAt" IS NULL
                """)
                .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;


            var replies = await _context.Database
                .SqlQuery<TicketReplyResponse>(
                $"""
                    SELECT 
                        tr."Id",
                        tr."AuthorId",
                        CONCAT(a."FirstName", ' ', a."LastName") AS "AuthorFullName",
                        tr."Content" AS "Message",
                        tr."CreatedAt"
                    FROM "TicketReplies" AS tr
                    INNER JOIN "Users" AS a
                    ON tr."AuthorId" = a."Id"
                    WHERE tr."TicketId" = {ticket.Id}
                    ORDER BY tr."CreatedAt" ASC
                """)
                .ToListAsync(ct);

            return new TicketDetailsResponse(ticket, replies);
        }


        // for supportagent and admin
        public async Task<TicketDetailsResponse?> GetTicketByIdAsync(Guid ticketId, CancellationToken ct)
        {
            var ticket = await _context.Database
               .SqlQuery<TicketResponse>(
               $"""
                    SELECT 
                        t."Id",
                        t."TicketNumber",
                        t."Title",
                        t."Description",
                        t."Status",
                        t."Priority",
                        c."Name" AS "CategoryName",
                        CONCAT(r."FirstName", ' ', r."LastName") AS "Requester",
                        CASE 
                            WHEN a."Id" IS NULL THEN NULL
                            ELSE CONCAT(a."FirstName", ' ', a."LastName")
                            END AS "AssignedAgent",
                        t."DueAt",
                        t."CreatedAt"
                    FROM "Tickets" AS t

                    INNER JOIN "Categories" AS c
                    ON t."CategoryId" = c."Id"

                    INNER JOIN "Users" As r
                    ON t."RequesterId" = r."Id"

                    LEFT JOIN "Users" AS a
                    ON t."AssignedAgentId" = a."Id"
                    
                    WHERE t."Id" = {ticketId} 
                    AND t."DeletedAt" IS NULL
                """)
               .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;

            var replies = await _context.Database
               .SqlQuery<TicketReplyResponse>(
               $"""
                    SELECT 
                        tr."Id",
                        tr."AuthorId",
                        CONCAT(a."FirstName", ' ', a."LastName") AS "AuthorFullName",
                        tr."Content" AS "Message",
                        tr."CreatedAt"
                    FROM "TicketReplies" AS tr
                    INNER JOIN "Users" AS a
                    ON tr."AuthorId" = a."Id"
                    WHERE tr."TicketId" = {ticket.Id}
                    ORDER BY tr."CreatedAt" ASC
                """)
               .ToListAsync(ct);

            return new TicketDetailsResponse(ticket, replies);
        }
    }
}
