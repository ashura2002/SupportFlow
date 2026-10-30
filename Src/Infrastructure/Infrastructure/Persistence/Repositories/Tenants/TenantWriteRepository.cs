using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Tenants
{
    public sealed class TenantWriteRepository : ITenantWriteRepository
    {
        private readonly SharedDbContext _context;
        public TenantWriteRepository(SharedDbContext context)
        {
            _context = context;
        }

        public void Add(Tenant tenant)
        {
            _context.Tenants.Add(tenant);
        }

        public async Task<Tenant?> GetMyTenantByIdAsync(
            Guid tenantAdminId,
            Guid tenantId,
            CancellationToken ct)
        {
            return await _context.Tenants
                .Where(t => t.Id == tenantId)
                .Where(t => _context.TenantMemberships
                .Any(m => m.TenantId == t.Id 
                && m.UserId == tenantAdminId 
                && m.Role == TenantRoles.TenantAdmin))
                .FirstOrDefaultAsync(ct);
        }
    }
}
