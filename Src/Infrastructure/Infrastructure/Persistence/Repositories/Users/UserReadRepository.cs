using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Domain.Enums;
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

        public async Task<IReadOnlyCollection<UserResponse>> GetTenantAdminsAsync(
            Guid tenantId,
            CancellationToken ct)
        {
            var query = from membership in _context.TenantMemberships.AsNoTracking()
                        join user in _context.Users.AsNoTracking()
                            on membership.UserId equals user.Id
                        join tenant in _context.Tenants.AsNoTracking()
                            on membership.TenantId equals tenant.Id
                        where membership.Role == TenantRoles.TenantAdmin
                             && membership.TenantId == tenantId
                        orderby user.FirstName, user.LastName
                        select new UserResponse(
                            user.Id,
                            user.FirstName,
                            user.LastName,
                            user.Email.Value,
                            membership.Role,
                            tenant.Name);

            return await query.ToListAsync(ct);

        }

        public async Task<PaginatedResult<UserResponse>> GetAllActiveUsersAsync(
            int page,
            int pageSize,
            Guid? tenantId,
            CancellationToken ct)
        {
            var query = _context.Users.AsNoTracking();

            if (tenantId.HasValue)
                query = query.Where(user =>
                _context.TenantMemberships.Any(membership =>
                membership.UserId == user.Id &&
                membership.TenantId == tenantId.Value));

            var totalCount = await query.CountAsync(ct);

            var activeUsers = await query
               .OrderBy(u => u.FirstName)
               .ThenBy(u => u.LastName)
               .Skip((page - 1) * pageSize)
               .Take(pageSize)
               .Select(u => new UserResponse(
                   u.Id,
                   u.FirstName,
                   u.LastName,
                   u.Email.Value,

                   _context.TenantMemberships
                        .Where(membership => membership.UserId == u.Id &&
                         (!tenantId.HasValue || membership.TenantId == tenantId.Value))
                        .Select(membership => (TenantRoles?)membership.Role)
                        .FirstOrDefault(),

                   (from membership in _context.TenantMemberships
                    join tenant in _context.Tenants
                        on membership.TenantId equals tenant.Id
                    where membership.UserId == u.Id &&
                        (!tenantId.HasValue || membership.TenantId == tenantId.Value)
                    select tenant.Name)
                        .FirstOrDefault()

                ))
               .ToListAsync(ct);


            return new PaginatedResult<UserResponse>(
                activeUsers,
                page,
                pageSize,
                totalCount);
        }

        public async Task<PaginatedResult<DeletedUserResponse>> GetAllDeletedUsersAsync(
            int page,
            int pageSize,
            Guid? tenantId,
            CancellationToken ct)
        {
            var query = _context.Users
               .AsNoTracking()
               .IgnoreQueryFilters()
               .Where(u => u.DeletedAt.HasValue);

            if (tenantId.HasValue)
                query = query.Where(u =>
                _context.TenantMemberships.Any(membership =>
                membership.UserId == u.Id
                && membership.TenantId == tenantId.Value));

            var totalCount = await query.CountAsync(ct);

            var deletedUsers = await query
                .OrderBy(u => u.FirstName)
                .ThenBy(u => u.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u => new DeletedUserResponse(
                u.Id,
                u.FirstName,
                u.LastName,
                u.Email.Value,

                _context.TenantMemberships
                        .Where(membership => membership.UserId == u.Id &&
                         (!tenantId.HasValue || membership.TenantId == tenantId.Value))
                        .Select(membership => (TenantRoles?)membership.Role)
                        .FirstOrDefault(),


                (
                    from membership in _context.TenantMemberships
                    join tenant in _context.Tenants
                        on membership.TenantId equals tenant.Id
                    where membership.UserId == u.Id &&
                       (!tenantId.HasValue || membership.TenantId == tenantId.Value)
                    select tenant.Name
                ).FirstOrDefault(),

                u.DeletedAt
                ))
                .ToListAsync(ct);


            return new PaginatedResult<DeletedUserResponse>(
                deletedUsers,
                page,
                pageSize,
                totalCount);
        }

        public async Task<UserResponse?> GetUserByIdAsync(
            Guid userId,
            Guid? tenantId,
            CancellationToken ct)
        {
            var query = _context.Users.AsNoTracking();

            // tenant admin restrict access to their own tenant.
            if (tenantId.HasValue)
            {
                query = query.Where(u => u.Id == userId &&
                _context.TenantMemberships.Any(membership =>
                membership.UserId == userId
                && membership.TenantId == tenantId.Value));
            }
            else
            {
                // platform admin platform-wide access.
                query = query.Where(u => u.Id == userId);
            }

            var user = await query
                .Select(u => new UserResponse(
                    u.Id,
                    u.FirstName,
                    u.LastName,
                    u.Email.Value,

                    _context.TenantMemberships
                        .Where(membership => membership.UserId == u.Id &&
                         (!tenantId.HasValue || membership.TenantId == tenantId.Value))
                        .Select(membership => (TenantRoles?)membership.Role)
                        .FirstOrDefault(),

                       (from membership in _context.TenantMemberships
                        join tenant in _context.Tenants
                            on membership.TenantId equals tenant.Id
                        where membership.UserId == u.Id &&
                          (!tenantId.HasValue || membership.TenantId == tenantId.Value)
                        select tenant.Name)
                        .FirstOrDefault()
                    ))
                    .FirstOrDefaultAsync(ct);

            return user;
        }

        public async Task<bool> IsEmailExist(string email, CancellationToken ct)
        {
            var emailVo = Email.Create(email);
            return await _context.Users.AsNoTracking()
                .AnyAsync(u => u.Email == emailVo, ct);
        }
    }
}