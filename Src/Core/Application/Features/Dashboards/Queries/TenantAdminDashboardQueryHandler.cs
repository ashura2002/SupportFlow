using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Dashboards.Queries
{
    public sealed class TenantAdminDashboardQueryHandler : IRequestHandler<TenantAdminDashboardQuery, Result<DashboardResponse>>
    {
        private readonly IDashboardRepository _dashboardRepository;
        private readonly ICurrentUserService _currentUserService;
        public TenantAdminDashboardQueryHandler(IDashboardRepository dashboardRepository, ICurrentUserService currentUserService)
        {
            _dashboardRepository = dashboardRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<DashboardResponse>> Handle(TenantAdminDashboardQuery request, CancellationToken cancellationToken)
        {
            var dashboard = await _dashboardRepository.GetDashboardResponseAsync(
                _currentUserService.TenantId,
                cancellationToken);

            return Result<DashboardResponse>.Success(dashboard);
        }
    }
}