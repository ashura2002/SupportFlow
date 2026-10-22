

using Domain.Enums;

namespace Application.ResponseDTO
{
    public sealed record TenantMembershipResponse(
        Guid Id,
        Guid UserId,
        Guid TenantId,
        TenantRoles TenantRoles,
        DateTime CreatedAt);
}
