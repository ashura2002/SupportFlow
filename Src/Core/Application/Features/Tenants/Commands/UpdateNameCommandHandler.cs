using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using MediatR;

namespace Application.Features.Tenants.Commands
{
    public sealed class UpdateNameCommandHandler : IRequestHandler<UpdateNameCommand, Result>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly ITenantWriteRepository _tenantWriteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateNameCommandHandler(
            ICurrentUserService currentUserService,
            ITenantWriteRepository tenantWriteRepository,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _tenantWriteRepository = tenantWriteRepository;
            _unitOfWork = unitOfWork;
        }


        public async Task<Result> Handle(UpdateNameCommand request, CancellationToken cancellationToken)
        {
            var tenant = await _tenantWriteRepository.GetMyTenantByIdAsync(
                _currentUserService.UserId,
                _currentUserService.TenantId,
                cancellationToken);

            if (tenant is null)
                return Result.Failure(TenantErrors.TenantNotFound);

            tenant.UpdateTenantName(request.TenantName);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
    }
}
