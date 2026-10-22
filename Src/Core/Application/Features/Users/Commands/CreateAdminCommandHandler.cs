
using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Users.Commands
{
    public sealed class CreateAdminCommandHandler : IRequestHandler<CreateAdminCommand, Result<Guid>>
    {
        private readonly ICurrentUserService _currentUserService;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IUserReadRepository _userReadRepository;
        private readonly ITenantMembershipWriteRepository _tenantMembershipWriteRepository;
        private readonly IPasswordService _passwordService;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAdminCommandHandler(
            ICurrentUserService currentUserService,
            IUserWriteRepository userWriteRepository,
            IUserReadRepository userReadRepository,
            ITenantMembershipWriteRepository tenantMembershipWriteRepository,
            IPasswordService passwordService,
            IUnitOfWork unitOfWork)
        {
            _currentUserService = currentUserService;
            _userWriteRepository = userWriteRepository;
            _userReadRepository = userReadRepository;
            _tenantMembershipWriteRepository = tenantMembershipWriteRepository;
            _passwordService = passwordService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        {
            var emailVo = Email.Create(request.Email);
            if (await _userReadRepository.IsEmailExist(
                request.Email, 
                cancellationToken))
                return Result<Guid>.Failure(UserErrors.EmailExist);

            var passwordHash = _passwordService.HashPassword(request.Password);

            var user = User.Create(
                request.FirstName,
                request.LastName,
                passwordHash,
                emailVo);

            var tenantMembership = TenantMembership.Create(
                user.Id,
                _currentUserService.TenantId,
                TenantRoles.TenantAdmin);

            _userWriteRepository.Add(user);
            _tenantMembershipWriteRepository.Add(tenantMembership);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(user.Id);
        }
    }
}
