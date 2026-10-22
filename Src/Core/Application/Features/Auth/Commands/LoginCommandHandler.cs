using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Domain.ValueObjects;
using MediatR;
using Domain.Enums;

namespace Application.Features.Auth.Command
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;
        private readonly ITenantMembershipReadRepository _tenantMembershipReadRepository;
        private readonly ITenantReadRepository _tenantReadRepository;

        public LoginCommandHandler(
            IUserWriteRepository userWriteRepository,
            IPasswordService passwordService,
            IJwtService jwtService,
            ITenantMembershipReadRepository tenantMembershipReadRepository,
            ITenantReadRepository tenantReadRepository)
        {
            _userWriteRepository = userWriteRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
            _tenantMembershipReadRepository = tenantMembershipReadRepository;
            _tenantReadRepository = tenantReadRepository;
        }

        public async Task<Result<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var email = Email.Create(request.Email);
            var user = await _userWriteRepository.GetUserByEmailAsync(email.Value, cancellationToken);

            if (user is null)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);

            var passwordMatch = _passwordService.VerifyPassword(request.Password, user.Password);
            if (!passwordMatch)
                return Result<AuthResponse>.Failure(AuthErrors.InvalidCredentials);

            // Platform Admin flow
            if (user.PlatformRole == PlatformRole.PlatformAdmin)
            {
                var platformToken =
                    _jwtService.GeneratePlatformAccessToken(user);

                return Result<AuthResponse>.Success(
                    new AuthResponse(platformToken));
            }

            // Regular tenant user flow
            var membership =
                await _tenantMembershipReadRepository
                    .GetUserTenantMembershipAsync(
                        user.Id,
                        cancellationToken);

            if (membership is null)
                return Result<AuthResponse>.Failure(
                    TenantMembershipErrors.TenantMembershipNotFound);

            var tenant =
                await _tenantReadRepository.GetActiveTenantByIdAsync(
                    membership.TenantId,
                    cancellationToken);

            if (tenant is null)
                return Result<AuthResponse>.Failure(
                    TenantErrors.TenantNotFound);

            var accessToken = _jwtService.GenerateAccessToken(
                user,
                tenant.Id,
                membership.TenantRoles);

            return Result<AuthResponse>.Success(
                new AuthResponse(accessToken));

        }
    }
}
