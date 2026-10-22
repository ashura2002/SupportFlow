using Domain.Entities;
using Domain.Enums;

namespace Application.Interfaces.Services
{
    public interface IJwtService
    {
        string GenerateAccessToken(
            User user,
            Guid tenantId,
            TenantRoles roles);

        string GeneratePlatformAccessToken(User user);
    }
}
