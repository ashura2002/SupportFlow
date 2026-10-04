using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Categories.Queries
{
    public sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<IReadOnlyCollection<CategoryResponse>>>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;
        public GetAllCategoriesQueryHandler(ICategoryReadRepository categoryReadRepository)
        {
            _categoryReadRepository = categoryReadRepository;
        }

        public async Task<Result<IReadOnlyCollection<CategoryResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryReadRepository.GetAllCategoriesAsync(cancellationToken);
            return Result<IReadOnlyCollection<CategoryResponse>>.Success(categories);
        }
    }
}
