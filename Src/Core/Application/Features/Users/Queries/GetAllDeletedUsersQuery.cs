using Application.Common.Results;
using Application.ResponseDTO;
using MediatR;

namespace Application.Features.Users.Queries;

public sealed record GetAllDeletedUsersQuery(int Page, int PageSize) : IRequest<Result<PaginatedResult<DeletedUserResponse>>>;
