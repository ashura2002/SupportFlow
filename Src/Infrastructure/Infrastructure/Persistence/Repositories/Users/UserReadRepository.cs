using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Users
{
    public sealed class UserReadRepository : IUserReadRepository
    {
        private readonly SupportFlowDbContext _context;
        public UserReadRepository(SupportFlowDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<UserResponse>> GetAllActiveUsersAsync(int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Users.AsNoTracking();

            var totalCount = await query.CountAsync(ct);

            var users = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new UserResponse(u.Id, u.FirstName, u.LastName, u.Email.Value, u.Role))
                .ToListAsync(ct);

            return new PaginatedResult<UserResponse>(
                users,
                page, 
                pageSize, 
                totalCount);
        }

        public async Task<UserResponse?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new UserResponse(
                    u.Id, 
                    u.FirstName, 
                    u.LastName, 
                    u.Email.Value,
                    u.Role))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsEmailExist(string email, CancellationToken ct)
        {
            var emailVo = Email.Create(email);
            return await _context.Users
                .AsNoTracking()
                .AnyAsync(u => u.Email == emailVo, ct);
        }
    }
}
