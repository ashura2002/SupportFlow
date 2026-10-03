
using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record ResolveTicketCommand(Guid TicketId) : IRequest<Result>;
}
