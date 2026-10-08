using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Notifications.Queries
{
    public sealed record GetAllNotificationQuery: IRequest<Result<IReadOnlyCollection<NotificationResponse>>>;
}
