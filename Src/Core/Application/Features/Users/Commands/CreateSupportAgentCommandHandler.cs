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
    public sealed class CreateSupportAgentCommandHandler : IRequestHandler<CreateSupportAgentCommand, Result<Guid>>
    {
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IUserReadRepository _userReadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordService _passwordService;
        private readonly ITenantMembershipWriteRepository _tenantMembershipWriteRepository;
        private readonly ICurrentUserService _currentUserService;

        public CreateSupportAgentCommandHandler(
            IUserWriteRepository userWriteRepository,
            IUserReadRepository userReadRepository,
            IUnitOfWork unitOfWork,
            IPasswordService passwordService,
            ITenantMembershipWriteRepository tenantMembershipWriteRepository,
            ICurrentUserService currentUserService)
        {
            _userWriteRepository = userWriteRepository;
            _userReadRepository = userReadRepository;
            _unitOfWork = unitOfWork;
            _passwordService = passwordService;
            _tenantMembershipWriteRepository = tenantMembershipWriteRepository;
            _currentUserService = currentUserService;
        }

        public async Task<Result<Guid>> Handle(CreateSupportAgentCommand request, CancellationToken cancellationToken)
        {
            var emailVo = Email.Create(request.Email);
            if (await _userReadRepository.IsEmailExist(emailVo.Value, cancellationToken))
                return Result<Guid>.Failure(UserErrors.EmailExist);

            var passwordHash = _passwordService.HashPassword(request.Password);
            var user = User.Create(
                request.FirstName,
                request.LastName,
                passwordHash,
                emailVo);

            var membership = TenantMembership.Create(
                user.Id,
                _currentUserService.TenantId,
                TenantRoles.SupportAgent);

            _userWriteRepository.Add(user);
            _tenantMembershipWriteRepository.Add(membership);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(user.Id);

        }
    }
}