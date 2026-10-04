using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Categories.Queries
{
    public sealed record GetAllCategoriesQuery : IRequest<Result<IReadOnlyCollection<CategoryResponse>>>;
}
