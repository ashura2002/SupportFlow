using Application.Common.Results;
using MediatR;

namespace Application.Features.Tenants.Commands
{
    public sealed record RegisterCompanyCommand(
        string TenantName,
        string Identifier,
        string FirstName,
        string LastName,
        string Password,
        string Email) : IRequest<Result<Guid>>;
}
