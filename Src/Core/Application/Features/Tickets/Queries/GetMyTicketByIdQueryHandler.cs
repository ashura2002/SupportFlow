using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetMyTicketByIdQueryHandler : IRequestHandler<GetMyTicketByIdQuery, Result<TicketDetailsResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITicketReadRepository _ticketReadRepository;
        public GetMyTicketByIdQueryHandler(ICurrentUserService currentUserService, ITicketReadRepository ticketReadRepository)
        {
            _currentUserService = currentUserService;
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<Result<TicketDetailsResponse>> Handle(GetMyTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var ticket = await _ticketReadRepository.GetMyTicketByIdAsync(request.TicketId, currentUserId, cancellationToken);
            if (ticket is null)
                return Result<TicketDetailsResponse>.Failure(TicketErrors.TicketNotFound);

            return Result<TicketDetailsResponse>.Success(ticket);
     
        }
    }
}
