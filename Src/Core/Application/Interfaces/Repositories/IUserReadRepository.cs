using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface IUserReadRepository
    {
        Task<bool> IsEmailExist(string email, CancellationToken ct);

        Task<UserResponse?> GetUserByIdAsync(
            Guid userId,
            Guid? tenantId,
            CancellationToken ct);

        Task<PaginatedResult<UserResponse>> GetAllActiveUsersAsync(
            int page, 
            int pageSize, 
            Guid? tenantId, 
            CancellationToken ct);

        Task<PaginatedResult<DeletedUserResponse>> GetAllDeletedUsersAsync(
            int page,
            int pageSize,
            Guid? tenantId,
            CancellationToken ct);

        Task<IReadOnlyCollection<UserResponse>> GetTenantAdminsAsync(
            Guid tenantId,
            CancellationToken ct);
    }
}
