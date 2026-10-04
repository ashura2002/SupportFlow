using Domain.Enums;
using Domain.Events;
using Domain.Exceptions;

namespace Domain.Entities
{
    public class Ticket : Aggregate
    {
        public string TicketNumber { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public TicketStatus Status { get; private set; }
        public Priority Priority { get; private set; }
        public Guid CategoryId { get; private set; }
        public Guid RequesterId { get; private set; }
        public Guid? AssignedAgentId { get; private set; }
        public DateTime? DueAt { get; private set; }
        public DateTime? DeletedAt { get; private set; }

        private Ticket(
            string ticketNumber,
            string title,
            string description,
            TicketStatus status,
            Priority priority,
            Guid categoryId,
            Guid requesterId,
            Guid? assignedAgentId,
            DateTime? dueAt)
        {
            TicketNumber = ticketNumber;
            Title = title;
            Description = description;
            Status = status;
            Priority = priority;
            CategoryId = categoryId;
            RequesterId = requesterId;
            AssignedAgentId = assignedAgentId;
            DueAt = dueAt;
        }

        public static Ticket Create(
           string ticketNumber,
           string title,
           string description,
           Priority priority,
           Guid categoryId,
           Guid requesterId)
        {

            ticketNumber = EnsureNotNullAndValid(ticketNumber);
            title = EnsureNotNullAndValid(title);
            description = EnsureNotNullAndValid(description);

            var ticket = new Ticket(
                ticketNumber,
                title,
                description,
                TicketStatus.Open,
                priority,
                categoryId,
                requesterId,
                null,
                null);
            ticket.AddEvent(new TicketCreatedDomainEvent(requesterId));
            return ticket;
        }

        public void SoftDelete()
        {
            if (DeletedAt.HasValue) return;
            DeletedAt = DateTime.UtcNow;
            Touch();
        }

        public void StartProgressAfterReopen()
        {
            if (Status != TicketStatus.Reopened)
                throw new DomainRuleViolationException("Only reopened tickets can resume progress.");

            Status = TicketStatus.InProgress;
            Touch();
        }

        public void Reopen()
        {
            if (Status != TicketStatus.Closed)
                throw new DomainRuleViolationException("Only closed tickets can be reopened.");
            Status = TicketStatus.Reopened;
            Touch();
        }

        public void Close()
        {
            if (Status != TicketStatus.Resolved)
                throw new DomainRuleViolationException("Only resolved tickets can be closed.");

            Status = TicketStatus.Closed;
            Touch();
        }

        public void Resolve()
        {
            if (Status != TicketStatus.InProgress)
                throw new DomainRuleViolationException("Only in-progress tickets can be resolved.");

            Status = TicketStatus.Resolved;
            AddEvent(new TicketResolvedDomainEvent(RequesterId));
            Touch();
        }

        public void ResumeProgress()
        {
            if (Status != TicketStatus.WaitingForUser)
                throw new DomainRuleViolationException("Only tickets in WaitingForUser status can resume progress.");

            Status = TicketStatus.InProgress;
            Touch();
        }

        public void WaitForUser()
        {
            if (Status != TicketStatus.InProgress)
                throw new DomainRuleViolationException("Only in-progress tickets can be moved to waiting for user.");

            Status = TicketStatus.WaitingForUser;
            Touch();
        }

        public void StartProgress()
        {
            if (Status != TicketStatus.Assigned)
                throw new DomainRuleViolationException("Only assigned tickets can be started.");

            if (!AssignedAgentId.HasValue)
                throw new DomainRuleViolationException("Ticket must have an assigned agent.");

            Status = TicketStatus.InProgress;
            Touch();
        }


        public void AssignAgent(Guid agentId)
        {
            if (agentId == Guid.Empty)
                throw new DomainRuleViolationException("Agent ID cannot be empty.");

            if (Status != TicketStatus.Open)
                throw new DomainRuleViolationException("Only open tickets can be assigned.");

            if (AssignedAgentId.HasValue)
                throw new DomainRuleViolationException("This ticket already has an assigned agent.");

            AssignedAgentId = agentId;
            Status = TicketStatus.Assigned;
            AddEvent(new SupportAgentAssignedDomainEvent(agentId, Id));
            Touch();
        }

        public void UpdateTitle(string ticketTitle)
        {
            EnsureTicketIsNotClosed("Closed tickets cannot update the title.");
            ticketTitle = EnsureNotNullAndValid(ticketTitle);
            if (Title == ticketTitle)
                return;

            Title = ticketTitle;
            Touch();
        }

        public void UpdateDescription(string desc)
        {
            EnsureTicketIsNotClosed("Closed tickets cannot update the description.");
            desc = EnsureNotNullAndValid(desc);
            if (Description == desc)
                return;

            Description = desc;
            Touch();
        }


        private static string EnsureNotNullAndValid(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new DomainRuleViolationException("Cannot be empty.");
            value = value.Trim();
            if (value.Length < 3)
                throw new DomainRuleViolationException("Content must be at least 3 characters long.");
            return value;
        }

        private void EnsureTicketIsNotClosed(string msg)
        {
            if (Status == TicketStatus.Closed)
                throw new DomainRuleViolationException(msg);
        }
    }
}
