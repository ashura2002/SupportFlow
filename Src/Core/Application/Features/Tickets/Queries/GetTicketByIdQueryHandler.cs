using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<TicketDetailsResponse>>
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetTicketByIdQueryHandler(ITicketReadRepository ticketReadRepository, ICurrentUserService currentUserService)
        {
            _ticketReadRepository = ticketReadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<TicketDetailsResponse>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketReadRepository.GetTicketByIdAsync(
                request.TicketId,
                _currentUserService.TenantId,
                cancellationToken);

            if (ticket is null)
                return Result<TicketDetailsResponse>.Failure(TicketErrors.TicketNotFound);

            return Result<TicketDetailsResponse>.Success(ticket);
        }
    }
}
