
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ITenantMembershipWriteRepository
    {
        void Add(TenantMembership tenantMembership);
    }
}
