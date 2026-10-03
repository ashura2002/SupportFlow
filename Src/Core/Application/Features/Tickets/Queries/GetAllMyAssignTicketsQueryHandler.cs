using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetAllMyAssignTicketsQueryHandler : IRequestHandler<GetAllMyAssignTicketsQuery, Result<PaginatedResult<TicketResponse>>>
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetAllMyAssignTicketsQueryHandler(ITicketReadRepository ticketReadRepository, ICurrentUserService currentUserService)
        {
            _ticketReadRepository = ticketReadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PaginatedResult<TicketResponse>>> Handle(GetAllMyAssignTicketsQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var tickets = await _ticketReadRepository.GetAllMyAssignedTicketsAsync(
                currentUserId, 
                request.Page, 
                request.PageSize, 
                cancellationToken);

            return Result<PaginatedResult<TicketResponse>>.Success(tickets);
        }
    }
}
