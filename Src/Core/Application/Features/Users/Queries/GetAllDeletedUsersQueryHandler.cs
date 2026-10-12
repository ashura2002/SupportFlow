using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries;

public sealed class GetAllDeletedUsersQueryHandler : IRequestHandler<GetAllDeletedUsersQuery, Result<IReadOnlyCollection<DeletedUserResponse>>>
{
    private readonly IUserReadRepository _userReadRepository;
    public GetAllDeletedUsersQueryHandler(IUserReadRepository userReadRepository)
    {
        _userReadRepository = userReadRepository;
    }

    public async Task<Result<IReadOnlyCollection<DeletedUserResponse>>> Handle(
        GetAllDeletedUsersQuery request,
         CancellationToken cancellationToken)
    {
        var users = await _userReadRepository.GetAllDeletedUsersAsync(cancellationToken);
        return Result<IReadOnlyCollection<DeletedUserResponse>>.Success(users);
    }
}
