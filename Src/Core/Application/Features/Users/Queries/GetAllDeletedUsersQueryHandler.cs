using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries;

public sealed class GetAllDeletedUsersQueryHandler : IRequestHandler<GetAllDeletedUsersQuery, Result<PaginatedResult<DeletedUserResponse>>>
{
    private readonly IUserReadRepository _userReadRepository;
    private readonly ICurrentUserService _currentUserService;
    public GetAllDeletedUsersQueryHandler(
        IUserReadRepository userReadRepository,
        ICurrentUserService currentUserService)
    {
        _userReadRepository = userReadRepository;
        _currentUserService = currentUserService;
    }

    public async Task<Result<PaginatedResult<DeletedUserResponse>>> Handle(
        GetAllDeletedUsersQuery request,
        CancellationToken cancellationToken)
    {
        Guid? tenantId = _currentUserService.IsPlatformAdmin ? null : _currentUserService.TenantId;

        var users = await _userReadRepository.GetAllDeletedUsersAsync(
            request.Page,
            request.PageSize,
            tenantId,
            cancellationToken);

        return Result<PaginatedResult<DeletedUserResponse>>.Success(users);
    }
}
