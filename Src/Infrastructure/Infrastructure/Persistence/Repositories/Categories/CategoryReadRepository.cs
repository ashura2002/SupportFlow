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

        public async Task<IReadOnlyCollection<CategoryResponse>> GetAllCategoriesAsync(CancellationToken ct)
        {
            return await _context.Categories
              .AsNoTracking()
              .OrderByDescending(c => c.CreatedAt)
              .Select(c => new CategoryResponse(
                  c.Id, 
                  c.Name, 
                  c.Description))
              .ToListAsync(ct);
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct)
        {
            return await _context.Categories
                .AsNoTracking()
                .Where(c => c.Id == categoryId)
                .Select(c => new CategoryResponse(c.Id, c.Name, c.Description))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsCategoryNameExist(string categoryName, Guid? excludeCategoryId, CancellationToken ct)
        {
            return await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Name == categoryName &&
                (excludeCategoryId == null || c.Id != excludeCategoryId), ct);
        }
    }
}
