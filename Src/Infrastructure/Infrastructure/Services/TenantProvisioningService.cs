using Application.Interfaces.Services;
using Infrastructure.Data;
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
            // Validated above against a strict whitelist pattern — safe from
            // injection. Schema names are identifiers, not values, so they
            // can't be parameterized the way ExecuteSqlAsync expects.
            ValidateSchemaName(schemaName);
            _tenantContext.SetTenant(schemaName);

            // create a real schema in postgresql
#pragma warning disable EF1002
            await _tenantDbContext.Database.ExecuteSqlRawAsync(
                $"CREATE SCHEMA IF NOT EXISTS \"{schemaName}\"", cancellationToken);
#pragma warning restore EF1002

            // get all sql pending tenant migrations (still tenant-template value) 
            var migrator = _tenantDbContext.GetService<IMigrator>();
            var script = migrator.GenerateScript();

            // replace placeholder schema to real schema name
            var tenantScript = script.Replace("tenant-template", schemaName);
            await _tenantDbContext.Database.ExecuteSqlRawAsync(tenantScript, cancellationToken);
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
