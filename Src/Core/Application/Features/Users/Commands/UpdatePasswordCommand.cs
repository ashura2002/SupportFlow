using Application.Common.Results;
using MediatR;

namespace Application.Features.Users.Commands
{
    public sealed record UpdatePasswordCommand(string NewPassword, string ConfirmNewPassword) : IRequest<Result>;
}
