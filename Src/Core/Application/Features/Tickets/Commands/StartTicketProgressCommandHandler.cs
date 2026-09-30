using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed class StartTicketProgressCommandHandler : IRequestHandler<StartTicketProgressCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartTicketProgressCommandHandler(
            ICurrentUserService currentUserService, 
            ITicketWriteRepository ticketWriteRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _ticketWriteRepository = ticketWriteRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(StartTicketProgressCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var ticket = await _ticketWriteRepository.GetTicketByIdAsync(request.TicketId, cancellationToken);

            if (ticket is null)
                return Result.Failure(TicketErrors.TicketNotFound);

            if (ticket.AssignedAgentId != currentUserId)
                return Result.Failure(TicketErrors.NotAssignedAgent);

            ticket.StartProgress();
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
