
using Domain.Entities;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryWriteRepository
    {
        void Add(Category category);
        Task<Category?> GetCategoryByIdAsync(
            Guid categoryId,  
            Guid tenantId,
            CancellationToken ct);
    }
}
