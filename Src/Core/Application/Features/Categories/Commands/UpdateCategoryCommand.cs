using Application.Common.Results;
using MediatR;

namespace Application.Features.Categories.Commands
{
    public sealed record UpdateCategoryCommand(
        Guid CategoryId,
        string Name,
        string? Description) : IRequest<Result>;
}
