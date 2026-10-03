using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record ReopenTicketCommand(Guid TicketId) : IRequest<Result>;
}
