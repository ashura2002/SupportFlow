using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Tenants.Queries
{
    public sealed class GetAllTenantsQueryHandler : IRequestHandler<GetAllTenantsQuery, Result<PaginatedResult<TenantResponse>>>
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        public GetAllTenantsQueryHandler(ITenantReadRepository tenantReadRepository)
        {
            _tenantReadRepository = tenantReadRepository;
        }

        public async Task<Result<PaginatedResult<TenantResponse>>> Handle(GetAllTenantsQuery request, CancellationToken cancellationToken)
        {
            var tenants = await _tenantReadRepository.GetAllTenantsAsync(request.Page, request.PageSize, cancellationToken);
            return Result<PaginatedResult<TenantResponse>>.Success(tenants);
        }
    }
}
