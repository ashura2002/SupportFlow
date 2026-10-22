using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetAllMyTicketsQueryHandler : IRequestHandler<GetAllMyTicketsQuery, Result<PaginatedResult<TicketResponse>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITicketReadRepository _ticketReadRepository;

        public GetAllMyTicketsQueryHandler(ICurrentUserService currentUserService, ITicketReadRepository ticketReadRepository)
        {
            _currentUserService = currentUserService;
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<Result<PaginatedResult<TicketResponse>>> Handle(GetAllMyTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketReadRepository.GetAllMyTicketsAsync(
                _currentUserService.UserId, 
                request.Page, 
                request.PageSize, 
                cancellationToken);

            return Result<PaginatedResult<TicketResponse>>.Success(tickets);
        }
    }
}
