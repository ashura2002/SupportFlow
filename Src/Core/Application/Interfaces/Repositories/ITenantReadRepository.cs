
using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface ITenantReadRepository
    {
        Task<TenantResponse?> GetActiveTenantByIdAsync(Guid id, CancellationToken ct);
        Task<bool> IsTenantIdentifierAlreadyExistAsync(string identifier, CancellationToken ct);
        Task<bool> IsTenantSchemaAlreadyExistAsync(string schema, CancellationToken ct);
        Task<PaginatedResult<TenantResponse>> GetAllTenantsAsync(int page, int pageSize, CancellationToken ct);
        Task<TenantResponse?> GetActiveTenantByIdentifierAsync(string identifier, CancellationToken ct);
    }
}
