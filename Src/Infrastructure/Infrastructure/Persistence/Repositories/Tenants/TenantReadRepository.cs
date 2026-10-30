using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Tenants
{
    public sealed class TenantReadRepository : ITenantReadRepository
    {
        private readonly SharedDbContext _context;

        public TenantReadRepository(SharedDbContext context)
        {
            _context = context;
        }

        public async Task<TenantResponse?> GetActiveTenantByIdentifierAsync(string identifier, CancellationToken ct)
        {
            return await _context.Tenants
                .AsNoTracking()
                .Where(t => t.Identifier == identifier && t.IsActive)
                .Select(t => new TenantResponse(
                    t.Id,
                    t.Name,
                    t.Identifier,
                    t.SchemaName,
                    t.IsActive,
                    t.CreatedAt))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<TenantResponse?> GetActiveTenantByIdAsync(Guid id, CancellationToken ct)
        {
            return await _context.Tenants
                .AsNoTracking()
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new TenantResponse(
                    t.Id,
                    t.Name,
                    t.Identifier,
                    t.SchemaName,
                    t.IsActive,
                    t.CreatedAt))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<PaginatedResult<TenantResponse>> GetAllTenantsAsync(int page, int pageSize, CancellationToken ct)
        {
            var query = _context.Tenants.AsNoTracking();
            var totalCount = await query.CountAsync(ct);

            var tenants = await query
                .OrderByDescending(t => t.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(t => new TenantResponse(t.Id, t.Name, t.Identifier, t.SchemaName, t.IsActive, t.CreatedAt))
                .ToListAsync(ct);

            return new PaginatedResult<TenantResponse>(
                tenants,
                page,
                pageSize,
                totalCount);
        }

        public async Task<bool> IsTenantIdentifierAlreadyExistAsync(string identifier, CancellationToken ct)
        {
            return await _context.Tenants
                .AsNoTracking()
                .AnyAsync(t => t.Identifier == identifier, ct);
        }

        public async Task<bool> IsTenantSchemaAlreadyExistAsync(string schema, CancellationToken ct)
        {
            return await _context.Tenants
              .AsNoTracking()
              .AnyAsync(t => t.SchemaName == schema, ct);
        }
    }
}
