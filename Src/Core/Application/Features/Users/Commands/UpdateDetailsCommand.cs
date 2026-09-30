
using Application.Common.Results;
using MediatR;

namespace Application.Features.Users.Commands
{
    public record UpdateDetailsCommand(string FirstName, string LastName) : IRequest<Result>;
}
