using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations
{
    internal sealed class TicketReplyConfiguration : IEntityTypeConfiguration<TicketReply>
    {
        public void Configure(EntityTypeBuilder<TicketReply> builder)
        {
            builder.HasKey(ticketReply => ticketReply.Id);

            builder.Property(ticketReply => ticketReply.TicketId)
             .IsRequired();

            builder.Property(ticketReply => ticketReply.AuthorId)
                .IsRequired();


            builder.HasOne<Ticket>()
                .WithMany()
                .HasForeignKey(ticketReply => ticketReply.TicketId)
                .OnDelete(DeleteBehavior.Restrict); ;

            builder.HasOne<User>()
                .WithMany()
                .HasForeignKey(ticketReply => ticketReply.AuthorId)
                .OnDelete(DeleteBehavior.Restrict); ;
        }
    }
}
