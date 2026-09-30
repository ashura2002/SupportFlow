using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Auth.Command
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;
}
