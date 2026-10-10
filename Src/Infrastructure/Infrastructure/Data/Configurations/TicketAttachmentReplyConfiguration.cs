using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations;

public class TicketAttachmentReplyConfiguration : IEntityTypeConfiguration<TicketAttachmentReply>
{
    public void Configure(EntityTypeBuilder<TicketAttachmentReply> builder)
    {
       builder.HasKey(ta => ta.Id);

       builder.Property(ta => ta.TicketReplyId)
              .IsRequired();

       builder.Property(ta => ta.PublicImageUrl)
              .IsRequired();

       builder.Property(ta => ta.PublicImageId)
              .IsRequired();

      builder.HasOne<TicketReply>()
             .WithMany(tr => tr.TicketAttachmentReplies)
             .HasForeignKey(ta => ta.TicketReplyId)
             .OnDelete(DeleteBehavior.Cascade);
    }
}
