using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetMyTicketByIdQueryHandler : IRequestHandler<GetMyTicketByIdQuery, Result<TicketResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITicketReadRepository _ticketReadRepository;
        public GetMyTicketByIdQueryHandler(ICurrentUserService currentUserService, ITicketReadRepository ticketReadRepository)
        {
            _currentUserService = currentUserService;
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<Result<TicketResponse>> Handle(GetMyTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var ticket = await _ticketReadRepository.GetMyTicketByIdAsync(request.TicketId, currentUserId, cancellationToken);
            if (ticket is null)
                return Result<TicketResponse>.Failure(TicketErrors.TicketNotFound);
            return Result<TicketResponse>.Success(ticket);
        }
    }
}
