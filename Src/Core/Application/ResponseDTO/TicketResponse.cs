
using Domain.Enums;

namespace Application.ResponseDTO
{
    public sealed record TicketResponse(
        Guid Id,
        string TicketNumber,
        string Title,
        string Description,
        TicketStatus Status,
        Priority Priority,
        string CategoryName,
        string Requester,
        string? AssignedAgent,
        DateTime? DueAt,
        DateTime CreatedAt);
}
