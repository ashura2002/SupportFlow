using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record StartTicketProgressCommand(Guid TicketId):IRequest<Result>;
}
