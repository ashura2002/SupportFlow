using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.ValueObjects;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Users
{
    internal sealed class UserWriteRepository : IUserWriteRepository
    {
        private readonly SupportFlowDbContext _context;

        public UserWriteRepository(SupportFlowDbContext context)
        {
            _context = context;
        }


        public void Add(User user)
        {
            _context.Users.Add(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email, CancellationToken ct)
        {
            var emailVo = Email.Create(email);

            return await _context.Users
                .Where(u => u.Email == emailVo)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId, ct);
        }

        public void Remove(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
