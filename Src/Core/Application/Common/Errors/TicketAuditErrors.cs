
using Domain.Enums;

namespace Application.Common.Errors;

public static class TicketAuditErrors
{
    public static readonly Error TicketAuditNotFound = new(
        "TicketAudit.NotFound",
        "Ticket audit not found.",
        ErrorType.NotFound);
}
