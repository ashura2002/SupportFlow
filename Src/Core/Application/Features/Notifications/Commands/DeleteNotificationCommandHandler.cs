using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Notifications.Commands
{
    public sealed class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationWriteRepository _notificationWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNotificationCommandHandler(ICurrentUserService currentUserService,
            INotificationWriteRepository notificationWriteRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _notificationWriteRepository = notificationWriteRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;

            var notification = await _notificationWriteRepository.GetNotificationByIdAsync(
                request.NotificationId, 
                currentUserId, 
                cancellationToken);

            if (notification is null)
                return Result.Failure(NotificationErrors.NotificationNotFound);

            _notificationWriteRepository.Remove(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
