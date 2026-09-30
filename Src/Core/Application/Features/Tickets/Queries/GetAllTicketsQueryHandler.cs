using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, Result<PaginatedResult<TicketResponse>>>
    {
        private readonly ITicketReadRepository _ticketReadRepository;

        public GetAllTicketsQueryHandler(ITicketReadRepository ticketReadRepository)
        {
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<Result<PaginatedResult<TicketResponse>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketReadRepository.GetAllTicketsAsync(request.Page, request.PageSize, cancellationToken);
            return Result<PaginatedResult<TicketResponse>>.Success(tickets);
        }
    }
}
