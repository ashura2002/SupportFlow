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
        private readonly ITenantUnitOfWork _tenantUnitOfWork;

        public StartTicketProgressCommandHandler(
            ICurrentUserService currentUserService, 
            ITicketWriteRepository ticketWriteRepository,
            ITenantUnitOfWork tenantUnitOfWork)
        {
            _currentUserService = currentUserService;
            _ticketWriteRepository = ticketWriteRepository;
            _tenantUnitOfWork = tenantUnitOfWork;
        }


        public async Task<Result> Handle(StartTicketProgressCommand request, CancellationToken cancellationToken)
        {

            var ticket = await _ticketWriteRepository.GetTicketByIdAsync(
                request.TicketId,
                _currentUserService.UserId,
                cancellationToken);

            if (ticket is null)
                return Result.Failure(TicketErrors.TicketNotFound);

            if (ticket.AssignedAgentId != _currentUserService.UserId)
                return Result.Failure(TicketErrors.NotAssignedAgent);

            ticket.StartProgress();
            await _tenantUnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
