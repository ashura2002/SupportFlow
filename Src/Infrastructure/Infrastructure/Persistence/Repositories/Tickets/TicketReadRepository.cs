using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Npgsql;


namespace Infrastructure.Persistence.Repositories.Tickets
{
    public sealed class TicketReadRepository : ITicketReadRepository
    {
        private readonly TenantDbContext _context;
        private readonly ITenantContext _tenantContext;
        public TicketReadRepository(TenantDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        // supportagent
        public async Task<PaginatedResult<TicketResponse>> GetAllMyAssignedTicketsAsync(Guid userId, int page, int pageSize, CancellationToken ct)
        {
#pragma warning disable EF1002
            var schema = ValidateSchemaName(_tenantContext.SchemaName);

            var offset = (page - 1) * pageSize;
            var tickets = await _context.Database
                .SqlQueryRaw<TicketResponse>(
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
                        FROM {schema}."Tickets" AS t

                        INNER JOIN {schema}."Categories" AS c
                        ON t."CategoryId" = c."Id"

                        INNER JOIN "Users" AS r
                        ON t."RequesterId" = r."Id"

                        LEFT JOIN "Users" AS a
                        ON t."AssignedAgentId" = a."Id"

                        WHERE t."AssignedAgentId" = @assignAgent
                        AND t."DeletedAt" IS NULL
                        ORDER BY t."CreatedAt" DESC
                        OFFSET @offset
                        LIMIT @pageSize
                """,
                new NpgsqlParameter("assignAgent", userId),
                new NpgsqlParameter("offset", offset),
                new NpgsqlParameter("pageSize", pageSize)
                )
                .ToListAsync(ct);

            var totalCount = await _context.Database
                .SqlQueryRaw<int>(
                $"""
                    SELECT COUNT (*) AS "Value"
                    FROM {schema}."Tickets"
                    WHERE "AssignedAgentId" = @assignAgent
                    AND "DeletedAt" IS NULL
                """,
                new NpgsqlParameter("assignAgent", userId))
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
            var schema = ValidateSchemaName(_tenantContext.SchemaName);
#pragma warning disable EF1002
            var totalCount = await _context.Database
                .SqlQueryRaw<int>(
                $"""
                    SELECT COUNT (*) AS "Value"
                    FROM {schema}."Tickets"
                    WHERE "RequesterId" = @requesterId
                    AND "DeletedAt" IS NULL
                """,
                new NpgsqlParameter("requesterId", userId))
                .SingleAsync(ct);

            var offset = (page - 1) * pageSize;

            var tickets = await _context.Database
                .SqlQueryRaw<TicketResponse>(
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
                    FROM {schema}."Tickets" AS t

                    INNER JOIN {schema}."Categories" AS c
                    ON t."CategoryId" = c."Id"

                    INNER JOIN "Users" AS r 
                    ON t."RequesterId" = r."Id"

                    LEFT JOIN "Users" AS a
                    ON t."AssignedAgentId" = a."Id"

                    WHERE t."RequesterId" = @requesterId 
                    AND t."DeletedAt" IS NULL
                    ORDER BY t."CreatedAt" DESC
                    OFFSET @offset
                    LIMIT @pageSize
                """,
                new NpgsqlParameter("requesterId", userId),
                new Npgsql.NpgsqlParameter("offset", offset),
                new Npgsql.NpgsqlParameter("pageSize", pageSize))
                .ToListAsync(ct);

            return new PaginatedResult<TicketResponse>(tickets, page, pageSize, totalCount);
        }

        public async Task<PaginatedResult<TicketResponse>> GetAllTicketsAsync(int page, int pageSize, CancellationToken ct)
        {
            var schema = ValidateSchemaName(_tenantContext.SchemaName);

// patanggal sa warning about sa sql injection
#pragma warning disable EF1002
            var totalCount = await _context.Database
                .SqlQueryRaw<int>(
                $"""
            SELECT COUNT(*) AS "Value"
            FROM "{schema}"."Tickets"
            WHERE "DeletedAt" IS NULL
        """)
                .SingleAsync(ct);

            var offset = (page - 1) * pageSize;


#pragma warning disable EF1002
            var tickets = await _context.Database
                .SqlQueryRaw<TicketResponse>(
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
            FROM "{schema}"."Tickets" AS t
            INNER JOIN {schema}."Categories" AS c ON t."CategoryId" = c."Id"
            INNER JOIN "Users" AS r ON t."RequesterId" = r."Id"
            LEFT JOIN "Users" AS a ON t."AssignedAgentId" = a."Id"
            WHERE t."DeletedAt" IS NULL
            ORDER BY t."CreatedAt" DESC
            OFFSET {offset}
            LIMIT {pageSize}
        """)
                .ToListAsync(ct);

            return new PaginatedResult<TicketResponse>(tickets, page, pageSize, totalCount);
        }

        // requester - owner of the ticket
        public async Task<TicketDetailsResponse?> GetMyTicketByIdAsync(Guid ticketId, Guid userId, CancellationToken ct)
        {
            var schema = ValidateSchemaName(_tenantContext.SchemaName);

            var ticket = await _context.Database
                .SqlQueryRaw<TicketResponse>(
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
                        FROM {schema}."Tickets" AS ticket
                        INNER JOIN {schema}."Categories" AS category
                        ON ticket."CategoryId" = category."Id"
                        INNER JOIN "Users" AS requester
                        ON ticket."RequesterId" = requester."Id"
                        LEFT JOIN "Users" AS agent
                        ON ticket."AssignedAgentId" = agent."Id"
                        WHERE ticket."Id" = @ticketId
                        AND ticket."RequesterId" = @userId
                        AND ticket."DeletedAt" IS NULL
                    """,
                    new NpgsqlParameter("ticketId", ticketId),
                    new NpgsqlParameter("userId", userId)
                )
                .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;

            var replies = await _context.Database
                .SqlQueryRaw<TicketReplyReadRow>(
                    $"""
                        SELECT 
                            ticketReply."Id",
                            ticketReply."AuthorId",
                            CONCAT(author."FirstName", ' ', author."LastName") AS "AuthorFullName",
                            ticketReply."Content" AS "Message",
                            ticketReply."CreatedAt"
                        FROM {schema}."TicketReplies" AS ticketReply
                        INNER JOIN "Users" AS author
                        ON ticketReply."AuthorId" = author."Id"
                        WHERE ticketReply."TicketId" = @ticketId
                        AND ticketReply."AuthorId" = @userId
                        ORDER BY ticketReply."CreatedAt" ASC
                    """,
                    new NpgsqlParameter("ticketId", ticket.Id),
                    new NpgsqlParameter("userId", userId)
                )
                .ToListAsync(ct);

            if (replies.Count == 0)
                return new TicketDetailsResponse(
                    ticket,
                    Array.Empty<TicketReplyResponse>());

            var attachments = await _context.Database
                .SqlQueryRaw<TicketReplyAttachmentReadRow>(
                    $"""
                        SELECT
                            ticketAttachment."TicketReplyId" AS "ReplyId",
                            ticketAttachment."PublicImageUrl",
                            ticketAttachment."PublicImageId"
                        FROM {schema}."TicketAttachmentReply" AS ticketAttachment
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
            var schema = ValidateSchemaName(_tenantContext.SchemaName);

            var ticket = await _context.Database
                .SqlQueryRaw<TicketResponse>(
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
                        FROM {schema}."Tickets" AS t
                        INNER JOIN {schema}."Categories" AS c
                        ON t."CategoryId" = c."Id"
                        INNER JOIN "Users" As r
                        ON t."RequesterId" = r."Id"
                        LEFT JOIN "Users" AS a
                        ON t."AssignedAgentId" = a."Id"
                        WHERE t."Id" = @ticketId 
                        AND t."DeletedAt" IS NULL
                    """,
                    new NpgsqlParameter("ticketId", ticketId)
                )
                .FirstOrDefaultAsync(ct);

            if (ticket is null)
                return null;


            var replies = await _context.Database
                .SqlQueryRaw<TicketReplyReadRow>(
                    $"""
                        SELECT 
                            tr."Id",
                            tr."AuthorId",
                            CONCAT(author."FirstName", ' ' , author."LastName") AS "AuthorFullName",
                            tr."Content" AS "Message",
                            tr."CreatedAt"
                        FROM {schema}."TicketReplies" AS tr
                        INNER JOIN "Users" AS author
                        ON tr."AuthorId" = author."Id"
                        WHERE tr."TicketId" = @ticketId
                        ORDER BY tr."CreatedAt" ASC
                    """,
                      new NpgsqlParameter("ticketId", ticket.Id)
                )
                .ToListAsync(ct);

            if (replies.Count == 0)
                return new TicketDetailsResponse(
                    ticket,
                    Array.Empty<TicketReplyResponse>());

            var attachments = await _context.Database
                .SqlQueryRaw<TicketReplyAttachmentReadRow>(
                    $"""
                            SELECT
                                ta."TicketReplyId" AS "ReplyId",
                                ta."PublicImageUrl",
                                ta."PublicImageId"
                            FROM {schema}."TicketAttachmentReply" AS ta
                            INNER JOIN "TicketReplies" AS tr
                            ON ta."TicketReplyId" = tr."Id"
                            WHERE tr."TicketId" = @ticketId
                        """,
                      new NpgsqlParameter("ticketId", ticket.Id)
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

        private static string ValidateSchemaName(string schemaName)
        {
            // Whitelist pattern: alphanumeric + underscore only, starts with letter
            if (string.IsNullOrWhiteSpace(schemaName) ||
                !Regex.IsMatch(schemaName, "^[a-zA-Z][a-zA-Z0-9_]*$"))
            {
                throw new InvalidOperationException($"Invalid schema name: {schemaName}");
            }
            return schemaName;
        }
    }
}


