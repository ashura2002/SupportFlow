using Application.Interfaces.Services;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Text.RegularExpressions;

namespace Infrastructure.Services
{
    public sealed class TenantProvisioningService : ITenantProvisioningService
    {
        private readonly TenantDbContext _tenantDbContext;
        private readonly ITenantContext _tenantContext;

        public TenantProvisioningService(TenantDbContext tenantDbContext, ITenantContext tenantContext)
        {
            _tenantDbContext = tenantDbContext;
            _tenantContext = tenantContext;
        }

        public async Task ProvisionAsync(string schemaName, CancellationToken cancellationToken)
        {
            ValidateSchemaName(schemaName);
            _tenantContext.SetTenant(schemaName);

            await using var transaction = await _tenantDbContext.Database
                .BeginTransactionAsync(cancellationToken);

            try
            {
#pragma warning disable EF1002
                await _tenantDbContext.Database.ExecuteSqlRawAsync(
                    $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\"", cancellationToken);
#pragma warning restore EF1002

                var migrator = _tenantDbContext.GetService<IMigrator>();

                // NoTransactions BEGIN/COMMIT
                // because we manage the transaction boundary here
                var script = migrator.GenerateScript(
                    options: MigrationsSqlGenerationOptions.NoTransactions);

                var tenantScript = script.Replace("tenant-template", schemaName);

                await _tenantDbContext.Database.ExecuteSqlRawAsync(tenantScript, cancellationToken);

#pragma warning disable EF1002
                await _tenantDbContext.Database.ExecuteSqlRawAsync(
                    $"""
                CREATE SEQUENCE IF NOT EXISTS "{schemaName}".ticket_number_seq
                START WITH 1 INCREMENT BY 1;
            """, cancellationToken);
#pragma warning restore EF1002

                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private static void ValidateSchemaName(string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                throw new ArgumentException("Schema name cannot be empty.", nameof(schemaName));

            if (!Regex.IsMatch(schemaName, @"^[a-z][a-z0-9_]*$"))
                throw new ArgumentException(
                    $"Invalid schema name '{schemaName}'.", nameof(schemaName));
        }
    }

}
