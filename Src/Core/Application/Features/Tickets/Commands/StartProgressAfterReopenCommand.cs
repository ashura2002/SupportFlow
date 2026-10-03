
using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record StartProgressAfterReopenCommand(Guid TicketId) : IRequest<Result>;
}
