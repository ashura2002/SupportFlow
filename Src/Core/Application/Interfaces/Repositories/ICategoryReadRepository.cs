
using Application.ResponseDTO;

namespace Application.Interfaces.Repositories
{
    public interface ICategoryReadRepository
    {
        Task<CategoryResponse?> GetCategoryByIdAsync(
            Guid categoryId, 
            Guid tenantId,
            CancellationToken ct);

        Task<bool> IsCategoryNameExist(
            string categoryName, 
            Guid tenantId,
            Guid? excludeCategoryId, 
            CancellationToken ct);

        Task<PaginatedResult<CategoryResponse>> GetAllCategoriesAsync(
            Guid tenantId,
            int page,
            int pageSize,
            CancellationToken ct);
    }
}
