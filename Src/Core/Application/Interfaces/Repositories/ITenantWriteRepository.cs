
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ITenantWriteRepository
    {
        void Add(Tenant tenant);
        Task<Tenant?> GetMyTenantByIdAsync(
            Guid tenantAdminId,
            Guid tenantId,
            CancellationToken ct);
    }
}
