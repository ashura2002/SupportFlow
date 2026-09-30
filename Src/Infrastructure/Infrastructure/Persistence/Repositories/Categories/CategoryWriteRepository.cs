using Application.Interfaces.Repositories;
using Domain.Entities;
using Infrastructure.Data;

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

        public void Remove(Category category)
        {
            _context.Categories.Remove(category);
        }
    }
}
