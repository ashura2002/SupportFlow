
using Domain.Enums;

namespace Application.Common.Errors
{
    public static class TicketErrors
    {
        public static readonly Error TicketNotFound = new("Ticket.NotFound", "Ticket not found.", ErrorType.NotFound);
        public static readonly Error NotAssignedAgent = new(
            "Ticket.NotAssignedAgent",
            "You are not the assigned agent of this ticket.",
            ErrorType.Forbidden);

        public static readonly Error NotTicketRequester = new("Ticket.NotTicketRequester", "You are not the requester of this ticket.", ErrorType.Forbidden);
    }
}
