using Application.Common.Results;
using MediatR;

namespace Application.Features.Tenants.Commands
{
    public sealed record UpdateNameCommand(string TenantName) : IRequest<Result>;
}
