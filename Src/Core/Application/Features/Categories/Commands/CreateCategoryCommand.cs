using Application.Common.Results;
using MediatR;

namespace Application.Features.Categories.Commands
{
    public sealed record CreateCategoryCommand(
        string Name, 
        string? Description):IRequest<Result<Guid>>;
}
