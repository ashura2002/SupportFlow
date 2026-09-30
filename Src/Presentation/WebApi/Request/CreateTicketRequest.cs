using Domain.Enums;

namespace WebApi.Request
{
    public sealed record CreateTicketRequest(
        string Title,
        string Description,
        Priority Priority,
        Guid CategoryId);
}
