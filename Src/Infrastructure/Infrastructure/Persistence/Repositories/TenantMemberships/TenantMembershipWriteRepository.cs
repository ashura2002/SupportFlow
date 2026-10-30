using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;


namespace Infrastructure.Persistence.Repositories.TenantMemberships
{
    public sealed class TenantMembershipWriteRepository : ITenantMembershipWriteRepository
    {
        private readonly SharedDbContext _context;

        public TenantMembershipWriteRepository(SharedDbContext context)
        {
            _context = context;
        }

        public void Add(TenantMembership tenantMembership)
        {
            _context.TenantMemberships.Add(tenantMembership);
        }
    }
}
