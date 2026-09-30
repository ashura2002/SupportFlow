using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries
{
    public sealed class GetMeQueryHandler : IRequestHandler<GetMeQuery, Result<UserResponse>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserReadRepository _userReadRepository;

        public GetMeQueryHandler(ICurrentUserService currentUserService, IUserReadRepository userReadRepository)
        {
            _currentUserService = currentUserService;
            _userReadRepository = userReadRepository;
        }

        public async Task<Result<UserResponse>> Handle(GetMeQuery request, CancellationToken cancellationToken)
        {
            var currentUserId = _currentUserService.UserId;
            var user = await _userReadRepository.GetUserByIdAsync(currentUserId, cancellationToken);

            if (user is null)
                return Result<UserResponse>.Failure(UserErrors.UserNotFound);

            return Result<UserResponse>.Success(user);
        }
    }
}
