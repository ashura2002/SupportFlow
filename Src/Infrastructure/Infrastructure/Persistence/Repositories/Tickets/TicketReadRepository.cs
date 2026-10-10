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
                            ticket."Id",
                            ticket."TicketNumber",
                            ticket."Title",
                            ticket."Description",
                            ticket."Status",
                            ticket."Priority",
                            category."Name" AS "CategoryName",
                            CONCAT(requester."FirstName", ' ', requester."LastName") AS "Requester",
                            CASE
                                WHEN agent."Id" IS NULL THEN NULL
                                ELSE CONCAT(agent."FirstName", ' ', agent."LastName")
                                END AS "AssignedAgent",
                            ticket."DueAt",
                            ticket."CreatedAt"
                        FROM "Tickets" AS ticket
                        INNER JOIN "Categories" AS category
                        ON ticket."CategoryId" = category."Id"
                        INNER JOIN "Users" AS requester
                        ON ticket."RequesterId" = requester."Id"
                        LEFT JOIN "Users" AS agent
                        ON ticket."AssignedAgentId" = agent."Id"
                        WHERE ticket."Id" = {ticketId}
                        AND ticket."RequesterId" = {userId}
                        AND ticket."DeletedAt" IS NULL
                    """
                )
                .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;

            var replies = await _context.Database
                .SqlQuery<TicketReplyReadRow>(
                    $"""
                        SELECT 
                            ticketReply."Id",
                            ticketReply."AuthorId",
                            CONCAT(author."FirstName", ' ', author."LastName") AS "AuthorFullName",
                            ticketReply."Content" AS "Message",
                            ticketReply."CreatedAt"
                        FROM "TicketReplies" AS ticketReply
                        INNER JOIN "Users" AS author
                        ON ticketReply."AuthorId" = author."Id"
                        WHERE ticketReply."TicketId" = {ticket.Id}
                        AND ticketReply."AuthorId" = {userId}
                        ORDER BY ticketReply."CreatedAt" ASC
                    """
                )
                .ToListAsync(ct);

            if (replies.Count == 0)
                return new TicketDetailsResponse(
                    ticket,
                    Array.Empty<TicketReplyResponse>());

            var attachments = await _context.Database
                .SqlQuery<TicketReplyAttachmentReadRow>(
                    $"""
                        SELECT
                            ticketAttachment."TicketReplyId" AS "ReplyId",
                            ticketAttachment."PublicImageUrl",
                            ticketAttachment."PublicImageId"
                        FROM "TicketAttachmentReply" AS ticketAttachment
                        INNER JOIN "TicketReplies" AS ticketReply
                        ON ticketAttachment."TicketReplyId" = ticketReply."Id"
                        ORDER By ticketAttachment."CreatedAt"
                    """
                )
                .ToListAsync(ct);

            var attachmentIds = attachments
                .GroupBy(a => a.ReplyId)
                .ToDictionary(
                    a => a.Key,
                    a => (IReadOnlyCollection<UploadedImageResult>)a
                        .Select(x =>
                        new UploadedImageResult(
                            x.PublicImageUrl,
                            x.PublicImageId))
                            .ToList()
                );

            var finalResponse = replies
                .Select(r => new TicketReplyResponse(
                    r.Id,
                    r.AuthorId,
                    r.AuthorFullName,
                    r.Message,
                    attachmentIds.TryGetValue(r.Id, out var replyAttachmentId)
                    ? replyAttachmentId
                    : Array.Empty<UploadedImageResult>(),
                    r.CreatedAt))
                    .ToList();

            return new TicketDetailsResponse(ticket, finalResponse);
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
                    """
                )
                .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;


            var replies = await _context.Database
                .SqlQuery<TicketReplyReadRow>(
                    $"""
                        SELECT 
                            tr."Id",
                            tr."AuthorId",
                            CONCAT(author."FirstName", ' ' , author."LastName") AS "AuthorFullName",
                            tr."Content" AS "Message",
                            tr."CreatedAt"
                        FROM "TicketReplies" AS tr
                        INNER JOIN "Users" AS author
                        ON tr."AuthorId" = author."Id"
                        WHERE tr."TicketId" = {ticket.Id}
                        ORDER BY tr."CreatedAt" ASC
                    """
                )
                .ToListAsync(ct);

            if (replies.Count == 0)
                return new TicketDetailsResponse(
                    ticket,
                    Array.Empty<TicketReplyResponse>());

            var attachments = await _context.Database
                .SqlQuery<TicketReplyAttachmentReadRow>(
                    $"""
                            SELECT
                                ta."TicketReplyId" AS "ReplyId",
                                ta."PublicImageUrl",
                                ta."PublicImageId"
                            FROM "TicketAttachmentReply" AS ta
                            INNER JOIN "TicketReplies" AS tr
                            ON ta."TicketReplyId" = tr."Id"
                            WHERE tr."TicketId" = {ticket.Id}
                        """
                )
                .ToListAsync(ct);

            var attachmentIds = attachments
                .GroupBy(x => x.ReplyId)
                .ToDictionary(
                    g => g.Key,
                    g => (IReadOnlyCollection<UploadedImageResult>)g
                        .Select(x =>
                        new UploadedImageResult(
                            x.PublicImageUrl,
                            x.PublicImageId))
                            .ToList()
                );

            var finalResponse = replies
                .Select(x => new TicketReplyResponse(
                    x.Id,
                    x.AuthorId,
                    x.AuthorFullName,
                    x.Message,
                    attachmentIds.TryGetValue(x.Id, out var attachmentReplies)
                    ? attachmentReplies
                    : Array.Empty<UploadedImageResult>(),
                    x.CreatedAt))
                    .ToList();

            return new TicketDetailsResponse(ticket, finalResponse);
        }
    }
}
