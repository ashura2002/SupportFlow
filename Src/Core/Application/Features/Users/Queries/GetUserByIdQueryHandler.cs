using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries
{
    public sealed class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, Result<UserResponse>>
    {
        private readonly IUserReadRepository _userReadRepository;

        public GetUserByIdQueryHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        public async Task<Result<UserResponse>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await _userReadRepository.GetUserByIdAsync(request.UserId, cancellationToken);
            if (user is null)
                return Result<UserResponse>.Failure(UserErrors.UserNotFound);

            return Result<UserResponse>.Success(user);
        }
    }
}
