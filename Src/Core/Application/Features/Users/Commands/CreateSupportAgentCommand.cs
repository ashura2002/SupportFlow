using Application.Common.Results;
using MediatR;

namespace Application.Features.Users.Commands
{
    public sealed record CreateSupportAgentCommand(
        string FirstName,
        string LastName,
        string Password,
        string Email): IRequest<Result<Guid>>;
}
