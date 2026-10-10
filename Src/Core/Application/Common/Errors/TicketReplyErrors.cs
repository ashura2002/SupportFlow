using Domain.Enums;

namespace Application.Common.Errors
{
    public static class TicketReplyErrors
    {
        public static readonly Error TicketReplyNotFound = new(
            "TicketReply.NotFound",
             "Ticket reply not found",
              ErrorType.NotFound);

        public static readonly Error NotAuthor = new(
            "TicketReply.NotAuthor",
             "You are not the author of this reply.",
              ErrorType.Forbidden);      
    }
}