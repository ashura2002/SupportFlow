using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories.Categories
{
    public sealed class CategoryWriteRepository : ICategoryWriteRepository
    {
        private readonly SupportFlowDbContext _context;
        public CategoryWriteRepository(SupportFlowDbContext context)
        {
            _context = context;
        }

        public void Add(Category category)
        {
            _context.Categories.Add(category);
        }

        public async Task<Category?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct)
        {
            return await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == categoryId, ct);
        }
    }
}
