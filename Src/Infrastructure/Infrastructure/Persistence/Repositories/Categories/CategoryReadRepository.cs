using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Categories
{
    public sealed class CategoryReadRepository : ICategoryReadRepository
    {
        private readonly SupportFlowDbContext _context;
        public CategoryReadRepository(SupportFlowDbContext context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<CategoryResponse>> GetAllCategoriesAsync(
            Guid tenantId,
            int page,
            int pageSize,
            CancellationToken ct)
        {
            var query = _context.Categories.AsNoTracking();
            var totalCount = await query
                .Where(c => c.TenantId == tenantId)
                .CountAsync(ct);

            var categories = await query
                .Where(c => c.TenantId == tenantId)
                .OrderByDescending(c => c.Name)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryResponse(
                    c.Id,
                    c.Name,
                    c.Description))
                .ToListAsync(ct);

            return new PaginatedResult<CategoryResponse>(
                categories,
                page,
                pageSize,
                totalCount);
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(
            Guid categoryId,
            Guid tenantId,
            CancellationToken ct)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == categoryId
                && c.TenantId == tenantId)
                .Select(c => new CategoryResponse(
                    c.Id,
                    c.Name,
                    c.Description))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsCategoryNameExist(
            string categoryName,
            Guid tenantId,
            Guid? excludeCategoryId,
            CancellationToken ct)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.TenantId == tenantId)
                .AnyAsync(c => c.Name == categoryName
                && (excludeCategoryId == null
                || c.Id != excludeCategoryId), ct);
        }
    }
}
