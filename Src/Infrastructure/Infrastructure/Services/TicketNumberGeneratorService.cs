using Application.Interfaces.Services;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    public sealed class TicketNumberGeneratorService : ITicketNumberGeneratorService
    {
        private readonly TenantDbContext _context;
        private readonly ITenantContext _tenantContext;

        public TicketNumberGeneratorService(
            TenantDbContext context,
            ITenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }

        public async Task<string> GenerateTicketNumber(CancellationToken ct)
        {
            var schemaName = _tenantContext.SchemaName;
            ValidateSchemaName(schemaName);

            // Schema name is an identifier, not a value — EF's FormattableString
            // interpolation (SqlQuery<T>($"...")) would wrongly parameterize it
            // as a quoted string literal. SqlQueryRaw with manual concatenation
            // is required here, same reasoning as TenantProvisioningService.
            var sql = $"SELECT nextval('\"{schemaName}\".ticket_number_seq') AS \"Value\"";

            var ticketNumber = await _context.Database
                .SqlQueryRaw<long>(sql)
                .SingleAsync(ct);   

            return $"SF-{ticketNumber:D6}";
        }

        private static void ValidateSchemaName(string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                throw new ArgumentException("Schema name cannot be empty.", nameof(schemaName));

            if (!Regex.IsMatch(schemaName, @"^[a-z][a-z0-9_]*$"))
                throw new ArgumentException($"Invalid schema name '{schemaName}'.", nameof(schemaName));
        }
    }
}