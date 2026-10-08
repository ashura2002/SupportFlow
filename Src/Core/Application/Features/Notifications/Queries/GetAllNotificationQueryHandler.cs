using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Notifications.Queries
{
    public sealed class GetAllNotificationQueryHandler : IRequestHandler<GetAllNotificationQuery, Result<IReadOnlyCollection<NotificationResponse>>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly INotificationReadRepository _notificationReadRepository;
        public GetAllNotificationQueryHandler(ICurrentUserService  currentUserService, INotificationReadRepository notificationReadRepository)
        {
            _currentUserService = currentUserService;
            _notificationReadRepository = notificationReadRepository;
        }

        public async Task<Result<IReadOnlyCollection<NotificationResponse>>> Handle(GetAllNotificationQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var notifications = await _notificationReadRepository.GetAllMyNotificationAsync(currentUserId, cancellationToken);

            return Result<IReadOnlyCollection<NotificationResponse>>.Success(notifications);
        }
    }
}
