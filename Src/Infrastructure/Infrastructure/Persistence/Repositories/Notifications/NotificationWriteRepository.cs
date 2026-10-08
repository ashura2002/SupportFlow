using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Notifications
{
    public sealed class NotificationWriteRepository : INotificationWriteRepository
    {
        private readonly SupportFlowDbContext _context;
        public NotificationWriteRepository(SupportFlowDbContext context)
        {
            _context = context;
        }


        public void Add(Notification notification)
        {
            _context.Notifications.Add(notification);
        }

        public async Task<Notification?> GetNotificationByIdAsync(Guid notificationId, Guid UserId, CancellationToken ct)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId 
                && n.UserId == UserId, ct);
        }

        public void Remove(Notification notification)
        {
            _context.Notifications.Remove(notification);
        }
    }
}
