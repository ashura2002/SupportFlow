using Domain.Exceptions;

namespace Domain.Entities
{
    public class TicketReply : BaseEntity
    {
        public Guid TicketId { get; private set; }
        public Guid AuthorId { get; private set; }
        public string Content { get; private set; }
        private List<TicketAttachmentReply> _ticketAttachmentReplies = new();
        public IReadOnlyCollection<TicketAttachmentReply> TicketAttachmentReplies => _ticketAttachmentReplies.AsReadOnly();

        private TicketReply(
            Guid ticketId,
            Guid authorId,
            string content)
        {
            TicketId = ticketId;
            AuthorId = authorId;
            Content = content;
        }

        public static TicketReply Create(Guid ticketId, Guid authorId, string content)
        {
            if (ticketId == Guid.Empty)
                throw new DomainRuleViolationException("Ticket ID is required.");

            if (authorId == Guid.Empty)
                throw new DomainRuleViolationException("Author ID is required.");

            if (string.IsNullOrWhiteSpace(content))
                throw new DomainRuleViolationException("Message cannot be empty.");

            content = content.Trim();

            return new TicketReply(
                ticketId,
                authorId,
                content);
        }

        public void AddAttachment(TicketAttachmentReply ticketAttachmentReply)
        {
            if (ticketAttachmentReply is null)
                throw new DomainRuleViolationException("Attachment cannot be null.");
            _ticketAttachmentReplies.Add(ticketAttachmentReply);
        }
    }
}
