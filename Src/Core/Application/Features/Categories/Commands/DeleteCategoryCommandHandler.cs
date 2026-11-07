using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Categories.Commands
{
    public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, Result>
    {
        private readonly ICategoryWriteRepository _categoryWriteRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ITenantUnitOfWork _tenantUnitOfWork;

        public DeleteCategoryCommandHandler(
            ICategoryWriteRepository categoryWriteRepository,
            ICurrentUserService currentUserService,
            ITenantUnitOfWork tenantUnitOfWork)
        {
            _categoryWriteRepository = categoryWriteRepository;
            _currentUserService = currentUserService;
            _tenantUnitOfWork = tenantUnitOfWork;
        }

        public async Task<Result> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
        {
            var category = await _categoryWriteRepository.GetCategoryByIdAsync(
                request.CategoryId,
                _currentUserService.TenantId,
                cancellationToken);
            if (category is null)
                return Result.Failure(CategoryErrors.CategoryNotFound);

            category.SoftDelete();
            await _tenantUnitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
