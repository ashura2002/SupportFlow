using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed class WaitForUserCommandHandler : IRequestHandler<WaitForUserCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly ITenantUnitOfWork _tenantUnitOfWork;

        public WaitForUserCommandHandler(
            ICurrentUserService currentUserService,
            ITicketWriteRepository ticketWriteRepository, 
            ITenantUnitOfWork tenantUnitOfWork)
        {
            _currentUserService = currentUserService;
            _ticketWriteRepository = ticketWriteRepository;
            _tenantUnitOfWork = tenantUnitOfWork;
        }

        public async Task<Result> Handle(WaitForUserCommand request, CancellationToken cancellationToken)
        {

            var ticket = await _ticketWriteRepository.GetTicketByIdAsync(
                request.TicketId, 
                _currentUserService.TenantId,
                cancellationToken);

            if (ticket is null)
                return Result.Failure(TicketErrors.TicketNotFound);

            if (ticket.AssignedAgentId != _currentUserService.UserId)
                return Result.Failure(TicketErrors.NotAssignedAgent);

            ticket.WaitForUser();
            await _tenantUnitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
