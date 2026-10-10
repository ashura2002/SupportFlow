using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Features.Tickets.Commands
{
    public sealed class ReplyToTicketCommandHandler : IRequestHandler<ReplyToTicketCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITicketWriteRepository _ticketWriteRepository;
        private readonly ITicketReplyWriteRepository _ticketReplyWriteRepository;
        private readonly IImageStorageService _imageStorageService;
        private readonly ILogger<ReplyToTicketCommandHandler> _logger;

        public ReplyToTicketCommandHandler(
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork,
            ITicketWriteRepository ticketWriteRepository,
            ITicketReplyWriteRepository ticketReplyWriteRepository,
            IImageStorageService imageStorageService,
            ILogger<ReplyToTicketCommandHandler> logger)
        {
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
            _ticketWriteRepository = ticketWriteRepository;
            _ticketReplyWriteRepository = ticketReplyWriteRepository;
            _imageStorageService = imageStorageService;
            _logger = logger;
        }


        public async Task<Result> Handle(ReplyToTicketCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var currentUserRole = _currentUserService.Role;

            var ticket = await _ticketWriteRepository.GetTicketByIdAsync(
                request.TicketId,
                cancellationToken);

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


            // para sa attachment, mga screenshots
            List<string> uploadedPublicIds = new();

            foreach (var file in request.Attachments)
            {
                var uploadResult = await _imageStorageService.UploadAsync(
                    file.Stream,
                    file.FileName,
                    file.ContentType,
                    cancellationToken);

                uploadedPublicIds.Add(uploadResult.PublicImageId);

                var attachment = TicketAttachmentReply.Create(
                    ticketReply.Id,
                    uploadResult.PublicImageUrl,
                    uploadResult.PublicImageId);

                ticketReply.AddAttachment(attachment);
            }

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                           ex,
                           "Failed to save ticket reply for TicketId {TicketId}. Rolling back {Count} uploaded file(s).",
                           ticket.Id,
                           uploadedPublicIds.Count);

                // delete orphaned files left in cloudinary storage
                foreach (var publicId in uploadedPublicIds)
                {
                    await RollbackImageAsync(publicId, ex, CancellationToken.None);
                }
                throw;
            }

            return Result.Success();
        }


        // No external CancellationToken param rollback must always run,
        // even if the original token is already cancelled.
        private async Task RollbackImageAsync(string publicImageId, Exception exception, CancellationToken ct)
        {
            try
            {
                await _imageStorageService.DeleteAsync(publicImageId, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to rollback uploaded image {imageurl}", publicImageId);
            }
        }
    }
}