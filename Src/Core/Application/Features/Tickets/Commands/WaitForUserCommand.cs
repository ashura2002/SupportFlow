
using Application.Common.Results;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed record WaitForUserCommand(Guid TicketId) : IRequest<Result>;
}
