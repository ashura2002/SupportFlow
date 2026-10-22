using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries
{
    public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
    {
        private readonly IUserReadRepository _userReadRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetUserByIdQueryHandler(
            IUserReadRepository userReadRepository,
            ICurrentUserService currentUserService)
        {
            _userReadRepository = userReadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            Guid? tenantId = _currentUserService.IsPlatformAdmin ? null : _currentUserService.TenantId;

            var user = await _userReadRepository.GetUserByIdAsync(
                request.UserId, 
                tenantId,
                cancellationToken);
            if (user is null)
                return Result<UserResponse>.Failure(UserErrors.UserNotFound);

            return Result<UserResponse>.Success(user);
        }
    }
}
