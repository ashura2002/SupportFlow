using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Categories.Queries
{
    public sealed record GetAllCategoriesQuery(int Page, int PageSize) : IRequest<Result<PaginatedResult<CategoryResponse>>>;
}
