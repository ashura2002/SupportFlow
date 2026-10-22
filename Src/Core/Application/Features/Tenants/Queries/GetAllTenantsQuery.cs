using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tenants.Queries
{
    public sealed record GetAllTenantsQuery(int Page, int PageSize) : IRequest<Result<PaginatedResult<TenantResponse>>>;
}
