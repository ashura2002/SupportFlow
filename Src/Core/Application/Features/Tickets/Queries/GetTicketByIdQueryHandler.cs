using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<TicketDetailsResponse>>
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        public GetTicketByIdQueryHandler(ITicketReadRepository ticketReadRepository)
        {
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<Result<TicketDetailsResponse>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketReadRepository.GetTicketByIdAsync(request.TicketId, cancellationToken);
            if (ticket is null)
                return Result<TicketDetailsResponse>.Failure(TicketErrors.TicketNotFound);

            return Result<TicketDetailsResponse>.Success(ticket);
        }
    }
}
