namespace Application.ResponseDTO;

public sealed record TicketReplyReadRow(
    Guid Id,
    Guid AuthorId,
    string AuthorFullName,
    string Message,
    DateTime CreatedAt
);
