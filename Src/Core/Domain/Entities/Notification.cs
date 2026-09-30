using Domain.Exceptions;

namespace Domain.Entities
{
    public class Notification : BaseEntity
    {
        public string Content { get; private set; }
        public bool IsRead { get; private set; }
        public Guid UserId { get; private set; }

        private Notification(string content, bool isRead, Guid userId)
        {
            Content = content;
            IsRead = isRead;
            UserId = userId;
        }

        public static Notification Create(string content, Guid userId, bool isRead = false)
        {
            if (userId == Guid.Empty)
                throw new DomainRuleViolationException("User ID is required.");

            if (string.IsNullOrWhiteSpace(content))
                throw new DomainRuleViolationException("Notification content cannot be empty.");

            return new Notification(content, isRead, userId);
        }


        public void Read()
        {
            if (IsRead)
                return;

            IsRead = true;
            Touch();
        }
    }
}
