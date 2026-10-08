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
            return await _context.Database
                 .SqlQuery<CategoryResponse>(
                   $"""
                    SELECT 
                        "Id",
                        "Name",
                        "Description"
                    FROM "Categories"
                    WHERE "DeletedAt" IS NULL
                   """)
                .ToListAsync(ct);
        }

        public async Task<CategoryResponse?> GetCategoryByIdAsync(Guid categoryId, CancellationToken ct)
        {
            return await _context.Database
                .SqlQuery<CategoryResponse>(
                $"""
                    SELECT 
                        "Id",
                        "Name",
                        "Description"
                    FROM "Categories"
                    WHERE "Id" = {categoryId} AND 
                    "DeletedAt" IS NULL
                """)
                .FirstOrDefaultAsync(ct);
        }

        public async Task<bool> IsCategoryNameExist(string categoryName, Guid? excludeCategoryId, CancellationToken ct)
        {
            return await _context.Database
                .SqlQuery<bool>(
                $"""
                    SELECT EXISTS (
                        SELECT 1
                        FROM "Categories"
                        WHERE "Name" = {categoryName}
                        AND "DeletedAt" IS NULL
                        AND (
                        {excludeCategoryId} IS NULL 
                        OR "Id" != {excludeCategoryId}
                        )
                    ) AS "Value"
                """).SingleAsync(ct);
        }
    }
}
