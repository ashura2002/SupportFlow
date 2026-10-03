using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed record GetMyTicketByIdQuery(Guid TicketId) : IRequest<Result<TicketDetailsResponse>>;
}
