using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record CloseTicketCommand(Guid TicketId) : IRequest<Result>;
}
