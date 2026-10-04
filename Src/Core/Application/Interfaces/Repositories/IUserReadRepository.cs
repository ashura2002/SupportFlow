using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface IUserReadRepository
    {
        Task<bool> IsEmailExist(string email, CancellationToken ct);
        Task<UserResponse?> GetUserByIdAsync(Guid userId, CancellationToken ct);
        Task<PaginatedResult<UserResponse>> GetAllActiveUsersAsync(int page, int pageSize, CancellationToken ct);
        Task<UserResponse?> GetAdminAsync(CancellationToken ct);
    }
}
