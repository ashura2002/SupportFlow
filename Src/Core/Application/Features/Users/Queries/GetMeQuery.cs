using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries
{
    public sealed record GetMeQuery : IRequest<Result<UserResponse>>;
}
