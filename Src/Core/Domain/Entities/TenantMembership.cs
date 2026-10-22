using Domain.Enums;
using Domain.Exceptions;

namespace Domain.Entities;

public class TenantMembership : BaseEntity
{
    public Guid UserId { get; private set; }
    public Guid TenantId { get; private set; }
    public TenantRoles Role { get; private set; }

    private TenantMembership(
        Guid userId,
        Guid tenantId,
        TenantRoles role)
    {
        UserId = userId;
        TenantId = tenantId;
        Role = role;
    }

    public static TenantMembership Create(
        Guid userId,
        Guid tenantId,
        TenantRoles role)
    {
        if (userId == Guid.Empty)
            throw new DomainRuleViolationException(
                "User ID cannot be empty.");

        if (tenantId == Guid.Empty)
            throw new DomainRuleViolationException(
                "Tenant ID cannot be empty.");
        if (!Enum.IsDefined(role))
            throw new DomainRuleViolationException("Invalid user role.");

        return new TenantMembership(userId, tenantId, role);
    }
}