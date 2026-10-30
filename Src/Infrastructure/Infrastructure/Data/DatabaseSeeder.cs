
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
        private readonly SharedDbContext _context;
        private readonly IPasswordService _passwordHasher;
        private readonly SeededUserSettings _seededUserSetting;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            SharedDbContext context,
            IPasswordService passwordHasher,
            IOptions<SeededUserSettings> options,
            ILogger<DatabaseSeeder> logger)
        {
            _context = context;
            _passwordHasher = passwordHasher;
            _seededUserSetting = options.Value;
            _logger = logger;
        }

        public async Task SeedAdminUser(
            CancellationToken cancellationToken = default)
        {

            bool existingUser = await _context.Users
                .AnyAsync(cancellationToken);

            if (existingUser)
                return;

            var email = Email.Create(_seededUserSetting.SEEDED_USER_EMAIL);
            var password = _passwordHasher.HashPassword(
                _seededUserSetting.SEEDED_USER_PASSWORD);

            var systemAdmin = User.CreatePlatformAdmin(
                _seededUserSetting.SEEDED_USER_FIRST_NAME,
                _seededUserSetting.SEEDED_USER_LAST_NAME,
                password,
                email);

            _context.Users.Add(systemAdmin);

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Seeded platform administrator: {AdminId}",
                systemAdmin.Id);
        }
    }
}