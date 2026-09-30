using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed record GetTicketByIdQuery(Guid TicketId) : IRequest<Result<TicketResponse>>;
}
