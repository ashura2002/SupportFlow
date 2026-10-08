
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface INotificationWriteRepository
    {
        void Add(Notification notification);
        void Remove(Notification notification);
        Task<Notification?> GetNotificationByIdAsync(Guid notificationId, Guid UserId, CancellationToken ct);
    }
}
