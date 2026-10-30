
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.TenantMemberships
{
    public sealed class TenantMembershipReadRepository : ITenantMembershipReadRepository
    {
        private readonly SharedDbContext _context;

        public TenantMembershipReadRepository(SharedDbContext context)
        {
            _context = context;
        }

        public async Task<TenantMembershipResponse?> GetUserTenantMembershipAsync(
        Guid userId,
        CancellationToken ct)
        {
            return await _context.TenantMemberships
                .AsNoTracking()
                .Where(tm => tm.UserId == userId)
                .Select(tm => new TenantMembershipResponse(
                    tm.Id,
                    tm.UserId,
                    tm.TenantId,
                    tm.Role,
                    tm.CreatedAt))
                .FirstOrDefaultAsync(ct);
        }
    }
}
