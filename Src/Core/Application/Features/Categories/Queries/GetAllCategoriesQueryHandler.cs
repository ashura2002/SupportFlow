using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Categories.Queries
{
    public sealed class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, Result<PaginatedResult<CategoryResponse>>>
    {
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetAllCategoriesQueryHandler(
            ICategoryReadRepository categoryReadRepository,
            ICurrentUserService currentUserService)
        {
            _categoryReadRepository = categoryReadRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<PaginatedResult<CategoryResponse>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryReadRepository.GetAllCategoriesAsync(
                request.Page,
                request.PageSize,
                cancellationToken);

            return Result<PaginatedResult<CategoryResponse>>.Success(categories);
        }
    }
}
