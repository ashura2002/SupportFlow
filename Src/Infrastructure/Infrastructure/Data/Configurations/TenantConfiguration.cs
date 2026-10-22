
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public sealed class TenantConfiguration : IEntityTypeConfiguration<Tenant>
    {
        public void Configure(EntityTypeBuilder<Tenant> builder)
        {
            // maps the Tenant entity to the shared table containing all registered tenants/organizations
            builder.ToTable("Tenants", "public");

            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(t => t.Identifier)
              .IsRequired()
              .HasMaxLength(63);

            builder.Property(t => t.SchemaName)
                .IsRequired()
                .HasMaxLength(63);

            builder.Property(t => t.IsActive)
                .IsRequired();

            builder.HasIndex(t => t.Identifier)
                .IsUnique();

            builder.HasIndex(t => t.SchemaName)
                .IsUnique();

        }
    }
}

