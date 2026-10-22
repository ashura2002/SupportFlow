using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, Result<PaginatedResult<TicketResponse>>>
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAllTicketsQueryHandler(ITicketReadRepository ticketReadRepository, ICurrentUserService currentUserService)
        {
            _ticketReadRepository = ticketReadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PaginatedResult<TicketResponse>>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
        {
            var tickets = await _ticketReadRepository.GetAllTicketsAsync(
                _currentUserService.TenantId,
                request.Page,
                request.PageSize, 
                cancellationToken);

            return Result<PaginatedResult<TicketResponse>>.Success(tickets);
        }
    }
}
