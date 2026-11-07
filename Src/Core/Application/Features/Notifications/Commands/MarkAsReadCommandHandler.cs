
using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Notifications.Commands
{
    public sealed class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationWriteRepository _notificationWriteRepository;
        private readonly ITenantUnitOfWork _tenantUnitOfWork;    
    
        public MarkAsReadCommandHandler(ICurrentUserService currentUserService, INotificationWriteRepository notificationWriteRepository,
            ITenantUnitOfWork tenantUnitOfWork)
        {
            _currentUserService = currentUserService;
            _notificationWriteRepository = notificationWriteRepository;
            _tenantUnitOfWork = tenantUnitOfWork;
        }


        public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var notification = await _notificationWriteRepository.GetNotificationByIdAsync(request.NotificationId, currentUserId, cancellationToken);

            if (notification is null)
                return Result.Failure(NotificationErrors.NotificationNotFound);

            notification.Read();
            await _tenantUnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
