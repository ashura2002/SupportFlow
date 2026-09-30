using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries
{
    public sealed class GetAllActiveUsersQueryHandler : IRequestHandler<GetAllActiveUsersQuery, Result<PaginatedResult<UserResponse>>>
    {
        private readonly IUserReadRepository _userReadRepository;
        public GetAllActiveUsersQueryHandler(IUserReadRepository userReadRepository)
        {
            _userReadRepository = userReadRepository;
        }

        public async Task<Result<PaginatedResult<UserResponse>>> Handle(GetAllActiveUsersQuery request, CancellationToken cancellationToken)
        {
            var user = await _userReadRepository.GetAllActiveUsersAsync(
                request.Page,
                request.PageSize, 
                cancellationToken);
            return Result<PaginatedResult<UserResponse>>.Success(user);
        }
    }
}
