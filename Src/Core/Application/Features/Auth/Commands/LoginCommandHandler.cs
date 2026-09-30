using Application.Common.Errors;
using Application.Common.Results;
using Application.Interfaces.Services;
using Application.Interfaces.Repositories;
using Application.ResponseDTO;
using Domain.Enums;
using Domain.ValueObjects;
using MediatR;

namespace Application.Features.Auth.Command
{
    public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponse>>
    {
        private readonly IUserWriteRepository _userWriteRepository;
        private readonly IPasswordService _passwordService;
        private readonly IJwtService _jwtService;

        public LoginCommandHandler(IUserWriteRepository userWriteRepository, IPasswordService passwordService, IJwtService jwtService)
        {
            _userWriteRepository = userWriteRepository;
            _passwordService = passwordService;
            _jwtService = jwtService;
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

            var accessToken = _jwtService.GenerateAccessToken(user);
            var authResponse = new AuthResponse(accessToken);

            return Result<AuthResponse>.Success(authResponse);
        }
    }
}
