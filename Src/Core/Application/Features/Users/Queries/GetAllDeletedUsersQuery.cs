using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries;

public sealed record GetAllDeletedUsersQuery : IRequest<Result<IReadOnlyCollection<DeletedUserResponse>>>;
