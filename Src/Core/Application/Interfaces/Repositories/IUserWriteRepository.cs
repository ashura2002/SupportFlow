using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface IUserWriteRepository
    {
        void Add(User user);
        void Remove(User user);
        Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct);
        Task<User?> GetUserByEmailAsync(string email, CancellationToken ct);
    }
}
