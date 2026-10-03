using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Features.Tickets.Commands
{
    public sealed class ReplyToTicketCommandHandler : IRequestHandler<ReplyToTicketCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly ITicketReplyWriteRepository _ticketReplyWriteRepository;

        public ReplyToTicketCommandHandler(ICurrentUserService currentUserService, IUnitOfWork unitOfWork, ITicketWriteRepository ticketWriteRepository,
            ITicketReplyWriteRepository ticketReplyWriteRepository)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _ticketWriteRepository = ticketWriteRepository;
            _ticketReplyWriteRepository = ticketReplyWriteRepository;
        }


        public async Task<Result> Handle(ReplyToTicketCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            var ticket = await _ticketWriteRepository.GetTicketByIdAsync(request.TicketId, cancellationToken);

            if (ticket is null)
                return Result.Failure(TicketErrors.TicketNotFound);


            if (currentUserRole == Roles.Requester &&
                ticket.RequesterId != currentUserId)
            {
                return Result.Failure(TicketErrors.NotTicketRequester);
            }


            if (currentUserRole == Roles.SupportAgent &&
                ticket.AssignedAgentId != currentUserId)
            {
                return Result.Failure(TicketErrors.NotAssignedAgent);
            }

            // requester and assigned SupportAgent can participate in the ticket conversation
            var ticketReply = TicketReply.Create(
                ticket.Id,
                currentUserId,
                request.Message);

            _ticketReplyWriteRepository.Add(ticketReply);

            // if the agent was waiting for the Requester, the requesters reply
            // allows the agent to continue working on the ticket
            if (currentUserRole == Roles.Requester && 
                ticket.Status == TicketStatus.WaitingForUser)
            {
                ticket.ResumeProgress();
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();

        }
    }
}
