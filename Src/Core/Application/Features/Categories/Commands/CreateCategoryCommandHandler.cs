using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using MediatR;

namespace Application.Features.Categories.Commands
{
    public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, Result<Guid>>
    {
        private readonly ICategoryWriteRepository _categoryWriteRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICategoryReadRepository _categoryReadRepository;
        private readonly ITenantUnitOfWork _tenantUnitOfWork;
        public CreateCategoryCommandHandler(
            ICategoryWriteRepository categoryWriteRepository,
            ICurrentUserService currentUserService,
            ICategoryReadRepository categoryReadRepository,
            ITenantUnitOfWork tenantUnitOfWork)
        {
            _categoryWriteRepository = categoryWriteRepository;
            _currentUserService = currentUserService;
            _categoryReadRepository = categoryReadRepository;
            _tenantUnitOfWork = tenantUnitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            if (await _categoryReadRepository.IsCategoryNameExist(
                request.Name, 
                null,
                cancellationToken))
                return Result<Guid>.Failure(CategoryErrors.CategoryNameExist);

            var category = Category.Create(
                request.Name,
                request.Description,
                _currentUserService.TenantId);
            _categoryWriteRepository.Add(category);
            await _tenantUnitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(category.Id);
        }
    }
}
