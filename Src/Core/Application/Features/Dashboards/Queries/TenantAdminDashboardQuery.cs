
using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Dashboards.Queries
{
    public sealed record TenantAdminDashboardQuery: IRequest<Result<DashboardResponse>>;
}
