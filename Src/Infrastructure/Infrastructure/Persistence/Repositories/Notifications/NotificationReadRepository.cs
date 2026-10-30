using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Notifications
{
    public sealed class NotificationReadRepository : INotificationReadRepository
    {
        private readonly SharedDbContext _context;
        public NotificationReadRepository(SharedDbContext context)
        {
            _context = context;
        }



        public async Task<IReadOnlyCollection<NotificationResponse>> GetAllMyNotificationAsync(Guid UserId, CancellationToken ct)
        {
            return await _context.Database
                .SqlQuery<NotificationResponse>(
                $"""
                    SELECT
                        "Id",
                        "Content",
                        "IsRead"
                    FROM "Notifications"
                    WHERE "UserId" = {UserId}
                    ORDER BY "CreatedAt"
                """)
                .ToListAsync(ct);
        }
    }
}
