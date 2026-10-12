
using Application.Common.Results;
using MediatR;

namespace Application.Features.Users.Commands
{
    public sealed record DeleteAccountCommand(string Email) : IRequest<Result>;
}
