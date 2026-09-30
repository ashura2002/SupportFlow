using Application.Common.Results;
using Domain.Enums;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record CreateTicketCommand(
        string Title,
        string Description,
        Priority Priority,
        Guid CategoryId) : IRequest<Result<Guid>>;
}
