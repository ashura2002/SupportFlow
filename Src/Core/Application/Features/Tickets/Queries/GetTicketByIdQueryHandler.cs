using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tickets.Queries
{
    public sealed class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, Result<TicketResponse>>
    {
        private readonly ITicketReadRepository _ticketReadRepository;
        public GetTicketByIdQueryHandler(ITicketReadRepository ticketReadRepository)
        {
            _ticketReadRepository = ticketReadRepository;
        }

        public async Task<Result<TicketResponse>> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketReadRepository.GetTicketByIdAsync(request.TicketId, cancellationToken);
            if (ticket is null)
                return Result<TicketResponse>.Failure(TicketErrors.TicketNotFound);

            return Result<TicketResponse>.Success(ticket);
        }
    }
}
