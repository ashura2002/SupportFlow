using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Dashboards;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly SharedDbContext _sharedContext;
    private readonly TenantDbContext _tenantContext;

    public DashboardRepository(
        SharedDbContext sharedContext,
        TenantDbContext tenantDbContext)
    {
        _sharedContext = sharedContext;
        _tenantContext = tenantDbContext;
    }

    public async Task<DashboardResponse> GetDashboardResponseAsync(
        Guid tenantId,
        CancellationToken ct)
    {
        var tenantUsers = _sharedContext.TenantMemberships
            .Where(m => m.TenantId == tenantId)
            .Join(_sharedContext.Users.IgnoreQueryFilters(),
              membership => membership.UserId,
              user => user.Id,
              (_, user) => user);

        var totalActiveUsers = await tenantUsers
            .Where(u => !u.DeletedAt.HasValue)
            .Select(u => u.Id)
            .Distinct()
            .CountAsync(ct);

        var totalUnActiveUsers = await tenantUsers
            .Where(u => u.DeletedAt.HasValue)
            .Select(u => u.Id)
            .Distinct()
            .CountAsync(ct);

        var tenantTickets = _tenantContext.Tickets
            .Where(t => !t.DeletedAt.HasValue);

        var totalTickets = await tenantTickets.CountAsync(ct);

        var totalUnAssignedTickets = await tenantTickets
            .CountAsync(t => t.AssignedAgentId == null, ct);

        return new DashboardResponse(
            totalActiveUsers,
            totalUnActiveUsers,
            totalTickets,
            totalUnAssignedTickets);
    }
}