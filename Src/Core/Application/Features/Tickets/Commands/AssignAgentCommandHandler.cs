using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Enums;
using MediatR;


namespace Application.Features.Tickets.Commands
{
    public sealed class AssignAgentCommandHandler : IRequestHandler<AssignAgentCommand, Result>
    {
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserReadRepository _userReadRepository;
        private readonly ICurrentUserService _currentUserService;

        public AssignAgentCommandHandler(
            ITicketWriteRepository ticketWriteRepository,
            IUnitOfWork unitOfWork, 
            IUserReadRepository userReadRepository,
            ICurrentUserService currentUserService)
        {
            _ticketWriteRepository = ticketWriteRepository;
            _unitOfWork = unitOfWork;
            _userReadRepository = userReadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result> Handle(AssignAgentCommand request, CancellationToken cancellationToken)
        {
            Guid? tenantId = _currentUserService.IsPlatformAdmin ? null : _currentUserService.TenantId;

            var agent = await _userReadRepository.GetUserByIdAsync(
                request.SupportAgentId,
                tenantId,
                cancellationToken);
            if (agent is null)
                return Result.Failure(UserErrors.UserNotFound);

            if (agent.Role != TenantRoles.SupportAgent)
                return Result.Failure(UserErrors.UserRoleNotAgent);

            var ticket = await _ticketWriteRepository.GetTicketByIdAsync(
                request.TicketId,
                _currentUserService.TenantId,
                cancellationToken);

            if (ticket is null)
                return Result.Failure(TicketErrors.TicketNotFound);

            ticket.AssignAgent(agent.Id);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
