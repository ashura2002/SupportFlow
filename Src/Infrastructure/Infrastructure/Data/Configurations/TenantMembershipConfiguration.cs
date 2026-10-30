using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    public sealed class TenantMembershipConfiguration : IEntityTypeConfiguration<TenantMembership>
    {
        public void Configure(EntityTypeBuilder<TenantMembership> builder)
        {
            builder.ToTable("TenantMemberships", "public");

            builder.HasKey(tm => tm.Id);

            builder.Property(tm => tm.TenantId)
                .IsRequired();

            builder.Property(tm => tm.UserId)
                .IsRequired();

            builder.Property(tm => tm.Role)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<Tenant>()
                .WithMany()
                .HasForeignKey(tm => tm.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(tm => new { tm.TenantId, tm.UserId })
                .IsUnique();
        }
    }
}
