

using Application.Common.Results;
using MediatR;

namespace Application.Features.Notifications.Commands
{
    public sealed record DeleteNotificationCommand(Guid NotificationId) : IRequest<Result>;
}
