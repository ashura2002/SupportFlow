using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Categories.Commands
{
    public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, Result>
    {
        private readonly ICategoryWriteRepository _categoryWriteRepository;
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateCategoryCommandHandler(
            ICategoryWriteRepository categoryWriteRepository, 
            ICategoryReadRepository categoryReadRepository,
            ICurrentUserService currentUserService,
            IUnitOfWork unitOfWork)
        {
            _categoryWriteRepository = categoryWriteRepository;
            _categoryReadRepository = categoryReadRepository;
            _currentUserService = currentUserService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryWriteRepository.GetCategoryByIdAsync(
                request.CategoryId,
                _currentUserService.TenantId,
                cancellationToken);
            if (category is null)
                return Result.Failure(CategoryErrors.CategoryNotFound);

            if (await _categoryReadRepository.IsCategoryNameExist(
                request.Name,
                _currentUserService.TenantId,
                category.Id,
                cancellationToken))
                return Result.Failure(CategoryErrors.CategoryNameExist);


            category.UpdateCategoryName(request.Name);
            category.UpdateDescription(request.Description);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
