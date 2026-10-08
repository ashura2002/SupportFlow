using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface INotificationReadRepository
    {
        Task<IReadOnlyCollection<NotificationResponse>> GetAllMyNotificationAsync(Guid userId, CancellationToken ct);
    }
}
