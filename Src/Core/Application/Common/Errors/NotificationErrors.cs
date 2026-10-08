

using Domain.Enums;

namespace Application.Common.Errors
{
    public static class NotificationErrors
    {
        public static readonly Error NotificationNotFound = new("Notification.NotFound", "Notification not found.", ErrorType.NotFound);
    }
}
