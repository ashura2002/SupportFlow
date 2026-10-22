

using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface IDashboardRepository
    {
        Task<DashboardResponse> GetDashboardResponseAsync(
            Guid tenantId,
            CancellationToken ct);
    }
}
