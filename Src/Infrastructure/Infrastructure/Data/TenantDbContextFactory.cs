using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Data
{
    public sealed class TenantDbContextFactory : IDesignTimeDbContextFactory<TenantDbContext>
    {

        public TenantDbContext CreateDbContext(string[] args)
        {
            // dotnet cli create this factory directly not through DI container
            // so we can't inject IConfiguration via constructor
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionBuilder = new DbContextOptionsBuilder<TenantDbContext>();
            optionBuilder.UseNpgsql(config.GetConnectionString("DefaultConnection"));


            // TenantDbContext requires an ITenantContext in its constructor
            // Normally the middleware sets this per request — but there's no
            // request here, just a CLI command. HasDefaultSchema() throws if
            // the schema name is empty, so we set a placeholder just to avoid
            // that error. This value is never used for real tenant data
            // it only exists to let EF generate the migration file.
            var tenantContext = new TenantContext();
            tenantContext.SetTenant("tenant-template");

            return new TenantDbContext(optionBuilder.Options, tenantContext);
        }
    }
}
