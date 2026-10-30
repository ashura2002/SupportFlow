using Domain.Entities;
using Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public sealed class SharedDbContext(DbContextOptions<SharedDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Tenant> Tenants => Set<Tenant>();
        public DbSet<TenantMembership> TenantMemberships => Set<TenantMembership>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");

            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new TenantConfiguration());
            modelBuilder.ApplyConfiguration(new TenantMembershipConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
