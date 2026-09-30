using Application.Interfaces.Services;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Infrastructure.Data
{
    public sealed class DatabaseSeeder
    {
        private readonly SupportFlowDbContext _context;
        private readonly IPasswordService _passwordhasher;
        private readonly SeededUserSettings _seededUserSetting;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            SupportFlowDbContext context,
            IPasswordService passwordhasher,
            IOptions<SeededUserSettings> options,
            ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _passwordhasher = passwordhasher;
            _seededUserSetting = options.Value;
            _logger = logger;
        }


        public async Task SeedAdminUser(CancellationToken cancellationToken = default)
        {
            bool existingUser = await _context.Users.AnyAsync(cancellationToken);
            if (existingUser) return;

            var firstName = _seededUserSetting.FirstName;
            var lastName = _seededUserSetting.LastName;
            var emailVo = Email.Create(_seededUserSetting.Email);
            string password = _passwordhasher.HashPassword(_seededUserSetting.Password);

            User admin = User.Create(firstName, lastName, password, emailVo, Roles.Administrator);

            _context.Users.Add(admin);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Seeding administrator... {admin}", admin.Id);
        }

    }
}
