using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed record GetAllMyAssignTicketsQuery(int Page, int PageSize) : IRequest<Result<PaginatedResult<TicketResponse>>>;
}
