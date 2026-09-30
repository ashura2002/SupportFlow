
using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryReadRepository
    {
        Task<CategoryResponse?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct);
        Task<bool> IsCategoryNameExist(string categoryName, CancellationToken ct);
    }
}
