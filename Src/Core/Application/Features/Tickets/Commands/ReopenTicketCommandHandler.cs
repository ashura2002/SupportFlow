using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed class ReopenTicketCommandHandler : IRequestHandler<ReopenTicketCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketWriteRepository _ticketWriteRepository;

        public ReopenTicketCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ITicketWriteRepository ticketWriteRepository)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _ticketWriteRepository = ticketWriteRepository;
        }

        public async Task<Result> Handle(ReopenTicketCommand request, CancellationToken cancellationToken)
        {
            var ticket = await _ticketWriteRepository.GetTicketByIdAsync(
                request.TicketId,
                _currentUserService.UserId,
                cancellationToken);
            if (ticket is null)
                return Result.Failure(TicketErrors.TicketNotFound);

            if (ticket.RequesterId != _currentUserService.UserId)
                return Result.Failure(TicketErrors.NotTicketRequester);

            ticket.Reopen();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
