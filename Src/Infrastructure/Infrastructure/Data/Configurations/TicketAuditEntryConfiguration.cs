using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public sealed class TicketAuditEntryConfiguration : IEntityTypeConfiguration<TicketAuditEntry>
{
    public void Configure(EntityTypeBuilder<TicketAuditEntry> builder)
    {
        builder.HasKey(t => t.Id);

        builder.Property(t => t.TicketId)
            .IsRequired();

        builder.Property(t => t.ActorUserId)
            .IsRequired(false);

        builder.Property(t => t.Action)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(t => t.OldValue)
            .HasConversion<int>()
            .IsRequired(false);

        builder.Property(t => t.NewValue)
            .HasConversion<int>()
            .IsRequired(false);

        builder.HasOne<Ticket>()
            .WithMany()
            .HasForeignKey(t => t.TicketId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Tenant>()
            .WithMany()
            .HasForeignKey(ta => ta.TenantId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

