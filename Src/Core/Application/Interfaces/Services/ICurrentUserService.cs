using Domain.Enums;

namespace Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        TenantRoles Role { get; }
        Guid TenantId { get; }
        bool IsPlatformAdmin { get; }
    }
}
