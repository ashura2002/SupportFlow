using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Tenants.Commands
{
    public sealed class RegisterCompanyCommandHandler : IRequestHandler<RegisterCompanyCommand, Result<Guid>>
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        private readonly IUserReadRepository _userReadRepository;
        private readonly ITenantWriteRepository _tenantWriteRepository;
        private readonly IPasswordService _passwordService;
        private readonly ITenantMembershipWriteRepository _tenantMembershipWriteRepository;
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly ISchemaNameGenerator _schemaNameGenerator;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCompanyCommandHandler(
            ITenantReadRepository tenantReadRepository,
            IUserReadRepository userReadRepository,
            ITenantWriteRepository tenantWriteRepository,
            IPasswordService passwordService,
            ITenantMembershipWriteRepository tenantMembershipWriteRepository,
            IUserWriteRepository userWriteRepository,
            ISchemaNameGenerator schemaNameGenerator,
            IUnitOfWork unitOfWork)
        {
            _tenantReadRepository = tenantReadRepository;
            _userReadRepository = userReadRepository;
            _tenantWriteRepository = tenantWriteRepository;
            _passwordService = passwordService;
            _tenantMembershipWriteRepository = tenantMembershipWriteRepository;
            _userWriteRepository = userWriteRepository;
            _schemaNameGenerator = schemaNameGenerator;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<Guid>> Handle(RegisterCompanyCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);

            if (await _tenantReadRepository.IsTenantIdentifierAlreadyExistAsync(request.Identifier, cancellationToken))
                return Result<Guid>.Failure(TenantErrors.TenantIdentifierAlreadyExists);

            if (await _userReadRepository.IsEmailExist(email.Value, cancellationToken))
                return Result<Guid>.Failure(UserErrors.EmailExist);

            var hashPassword = _passwordService.HashPassword(request.Password);

            var initialAdmin = User.Create(
                request.FirstName,
                request.LastName,
                hashPassword,
                email);

            var schemaName = _schemaNameGenerator.Generate(request.Identifier);
            if (await _tenantReadRepository.IsTenantSchemaAlreadyExistAsync(schemaName, cancellationToken))
                return Result<Guid>.Failure(TenantErrors.SchemaAlreadyExists);

            var tenant = Tenant.Create(
                request.TenantName,
                request.Identifier,
                schemaName);

            var tenantMembership = TenantMembership.Create(
                initialAdmin.Id,
                tenant.Id,
                TenantRoles.TenantAdmin);

            _userWriteRepository.Add(initialAdmin);
            _tenantWriteRepository.Add(tenant);
            _tenantMembershipWriteRepository.Add(tenantMembership);

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result<Guid>.Success(tenant.Id);
        }
    }
}