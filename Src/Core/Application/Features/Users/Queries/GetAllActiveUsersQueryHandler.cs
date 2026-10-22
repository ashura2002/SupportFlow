using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries
{
    public sealed class GetAllActiveUsersQueryHandler : IRequestHandler<GetAllActiveUsersQuery, Result<PaginatedResult<UserResponse>>>
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly ICurrentUserService _currentUserService;
        public GetAllActiveUsersQueryHandler(
            IUserReadRepository userReadRepository,
            ICurrentUserService currentUserService)
        {
            _userReadRepository = userReadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PaginatedResult<UserResponse>>> Handle(GetAllActiveUsersQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = _currentUserService.IsPlatformAdmin 
                ? null 
                : _currentUserService.TenantId;

            var user = await _userReadRepository.GetAllActiveUsersAsync(
                request.Page,
                request.PageSize, 
                tenantId,
                cancellationToken);
            return Result<PaginatedResult<UserResponse>>.Success(user);
        }
    }
}
