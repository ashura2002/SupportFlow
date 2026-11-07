using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Text.RegularExpressions;

namespace Infrastructure.Persistence.Repositories.Notifications
{
    public sealed class NotificationReadRepository : INotificationReadRepository
    {
        private readonly TenantDbContext _context;
        private readonly ITenantContext _tenantContext;

        public NotificationReadRepository(TenantDbContext context, ITenantContext tenantContext)
        {
            _context = context;
            _tenantContext = tenantContext;
        }



        public async Task<IReadOnlyCollection<NotificationResponse>> GetAllMyNotificationAsync(Guid userId, CancellationToken ct)
        {
            var schema = ValidateSchemaName(_tenantContext.SchemaName);

#pragma warning disable EF1002
            return await _context.Database
                .SqlQueryRaw<NotificationResponse>(
                $"""
                    SELECT
                        "Id",
                        "Content",
                        "IsRead"
                    FROM {schema}."Notifications"
                    WHERE "UserId" = @userId
                    ORDER BY "CreatedAt"
                """,
                new NpgsqlParameter("userId", userId))
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
}
