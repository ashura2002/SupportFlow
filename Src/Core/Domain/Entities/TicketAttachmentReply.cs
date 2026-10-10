using Domain.Exceptions;

namespace Domain.Entities
{
    public class TicketAttachmentReply : BaseEntity
    {
        public Guid TicketReplyId { get; private set; }
        public string PublicImageUrl { get; private set; }
        public string PublicImageId { get; private set; }

        private TicketAttachmentReply(
            Guid ticketReplyId,
             string publicImageUrl,
              string publicImageId)
        {
            TicketReplyId = ticketReplyId;
            PublicImageId = publicImageId.Trim();
            PublicImageUrl = publicImageUrl.Trim();
        }


        public static TicketAttachmentReply Create(
            Guid ticketReplyId,
            string publicImageUrl,
            string publicImageId
        )
        {
            if (ticketReplyId == Guid.Empty)
                throw new DomainRuleViolationException("Ticket reply id is required.");

            if (string.IsNullOrWhiteSpace(publicImageUrl))
                throw new DomainRuleViolationException("Image URL is required.");

            if (string.IsNullOrWhiteSpace(publicImageId))
                throw new DomainRuleViolationException("Public image id cannot be empty.");

            return new TicketAttachmentReply(
                ticketReplyId,
                publicImageUrl,
                publicImageId);
        }
    }
}
