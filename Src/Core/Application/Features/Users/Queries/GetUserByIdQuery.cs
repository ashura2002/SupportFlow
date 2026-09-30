using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries
{
    public sealed record GetUserByIdQuery(Guid UserId) : IRequest<Result<UserResponse>>;
}
