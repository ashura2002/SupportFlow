using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Categories
{
    public sealed class CategoryWriteRepository : ICategoryWriteRepository
    {
        private readonly TenantDbContext _context;
        public CategoryWriteRepository(TenantDbContext context)
        {
            _context = context;
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        public async Task<Category?> GetCategoryByIdAsync(
            Guid categoryId,
            Guid tenantId,
            CancellationToken ct)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        }
    }
}
