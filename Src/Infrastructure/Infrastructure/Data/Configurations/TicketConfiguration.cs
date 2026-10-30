using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    internal sealed class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.HasKey(ticket => ticket.Id);

            builder.Property(ticket => ticket.TicketNumber)
                 .IsRequired();

            builder.Property(ticket => ticket.Title)
                 .IsRequired();

            builder.Property(ticket => ticket.Description)
                .IsRequired();

            builder.Property(ticket => ticket.Status)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(ticket => ticket.Priority)
                 .HasConversion<int>()
                 .IsRequired();

            builder.Property(ticket => ticket.CategoryId)
                .IsRequired();

            builder.Property(ticket => ticket.RequesterId)
                .IsRequired();

            builder.Property(ticket => ticket.AssignedAgentId)
                .IsRequired(false);

            builder.Property(ticket => ticket.DueAt)
                .IsRequired(false);

            builder.HasIndex(ticket => ticket.TicketNumber)
                .IsUnique();


            builder.HasOne<Category>()
                .WithMany()
                .HasForeignKey(t => t.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasQueryFilter(ticket => ticket.DeletedAt == null);

            builder.HasIndex(ticket => ticket.AssignedAgentId);

            builder.HasIndex(ticket => ticket.RequesterId);
        }
    }
}
