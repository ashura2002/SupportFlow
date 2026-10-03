namespace Application.ResponseDTO
{
    public sealed record TicketDetailsResponse(
       TicketResponse Ticket,
       IReadOnlyList<TicketReplyResponse> Replies);
}
