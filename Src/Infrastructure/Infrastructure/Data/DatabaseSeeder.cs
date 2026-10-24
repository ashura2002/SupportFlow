
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
        private readonly IPasswordService _passwordHasher;
        private readonly SeededUserSettings _seededUserSetting;
        private readonly ILogger<DatabaseSeeder> _logger;

        public DatabaseSeeder(
            SupportFlowDbContext context,
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
            Console.WriteLine("SEEDED USER EMAIL "+ _seededUserSetting.SEEDED_USER_EMAIL);


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


        public async Task SeedDemoTenantData(
            CancellationToken cancellationToken = default)
        {
            const string tenantIdentifier = "supportflow-demo";
            const string schemaName = "tenant_supportflow_demo";

            // 1. Find or create the demo tenant.
            var tenant = await _context.Tenants
                .FirstOrDefaultAsync(
                    t => t.Identifier == tenantIdentifier,
                    cancellationToken);

            if (tenant is null)
            {
                tenant = Tenant.Create(
                    "SupportFlow Demo",
                    tenantIdentifier,
                    schemaName);

                _context.Tenants.Add(tenant);

                await _context.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Seeded demo tenant: {TenantId}",
                    tenant.Id);
            }

            await SeedTenantUser(
                tenant.Id,
                "Demo",
                "Admin",
                "admin@demo.com",
                "admin12345",
                TenantRoles.TenantAdmin,
                cancellationToken);

            // 2. Seed requester.
            await SeedTenantUser(
                tenant.Id,
                "Demo",
                "Requester",
                "requester@demo.com",
                "requester12345",
                TenantRoles.Requester,
                cancellationToken);

            // 3. Seed support agent.
            await SeedTenantUser(
                tenant.Id,
                "Demo",
                "Agent",
                "agent@demo.com",
                "agent12345",
                TenantRoles.SupportAgent,
                cancellationToken);
        }

        private async Task SeedTenantUser(
            Guid tenantId,
            string firstName,
            string lastName,
            string emailAddress,
            string plainPassword,
            TenantRoles role,
            CancellationToken cancellationToken)
        {
            var email = Email.Create(emailAddress);

            // Find an existing global user by email.
            var user = await _context.Users
                .FirstOrDefaultAsync(
                    u => u.Email == email,
                    cancellationToken);

            if (user is null)
            {
                var passwordHash =
                    _passwordHasher.HashPassword(plainPassword);

                user = User.Create(
                    firstName,
                    lastName,
                    passwordHash,
                    email);

                _context.Users.Add(user);
            }

            // Ensure the user has a membership in this tenant.
            var membershipExists = await _context.TenantMemberships
                .AnyAsync(
                    m => m.UserId == user.Id &&
                         m.TenantId == tenantId,
                    cancellationToken);

            if (!membershipExists)
            {
                var membership = TenantMembership.Create(
                    user.Id,
                    tenantId,
                    role);

                _context.TenantMemberships.Add(membership);
            }

            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Ensured demo membership for {Email} in tenant {TenantId}",
                emailAddress,
                tenantId);
        }
    }
}