
using Domain.Enums;

namespace Application.Common.Errors
{
    public static class TenantMembershipErrors
    {
        public static readonly Error TenantMembershipNotFound = new(
            "TenantMembership.NotFound",
            "Tenant membership not found.",
            ErrorType.NotFound);
    }
}
