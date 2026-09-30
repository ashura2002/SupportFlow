using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record AssignAgentCommand(Guid SupportAgentId, Guid TicketId): IRequest<Result>;
}
