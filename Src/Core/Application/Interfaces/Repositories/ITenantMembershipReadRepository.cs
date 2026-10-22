using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface ITenantMembershipReadRepository
    {
        Task<TenantMembershipResponse?> GetUserTenantMembershipAsync(Guid userId, CancellationToken ct);
    }
}
