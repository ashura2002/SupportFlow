using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.RegularExpressions;

namespace Infrastructure.Persistence.Repositories.TicketAuditEntry;

public sealed class TicketAuditReadRepository : ITicketAuditReadRepository
{
    private readonly TenantDbContext _context;
    private readonly ITenantContext _tenantContext;
    public TicketAuditReadRepository(TenantDbContext context, ITenantContext tenantContext)
    {
        _context = context;
        _tenantContext = tenantContext;
    }
    public async Task<IReadOnlyList<TicketAuditResponse>> GetTicketAuditByIdAsync(
        Guid ticketId, 
        CancellationToken ct)
    {
        var schema = ValidateSchemaName(_tenantContext.SchemaName);

#pragma warning disable EF1002
        return await _context.Database
            .SqlQueryRaw<TicketAuditResponse>(
            $"""
                SELECT 
                    ta."Id",
                    ta."ActorUserId",
                    ta."Action",
                    ta."OldValue",
                    ta."NewValue",
                    ta."CreatedAt"
                FROM {schema}."TicketAuditEntries" AS ta
                WHERE ta."TicketId" = @ticketId
            """,
            new NpgsqlParameter("ticketId", ticketId))
            .ToListAsync(ct);
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
