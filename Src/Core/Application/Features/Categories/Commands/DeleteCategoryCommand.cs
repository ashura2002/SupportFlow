using Application.Common.Results;
using MediatR;

namespace Application.Features.Categories.Commands
{
    public sealed record DeleteCategoryCommand(Guid CategoryId) : IRequest<Result>;
}
