using Application.Interfaces.Services;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Infrastructure.Services
{
    public sealed class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }


        public Guid UserId
        {
            get
            {
                var userId = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userId, out var id))
                    throw new UnauthorizedAccessException("User ID claim is missing.");

                return id;
            }
        }

        public TenantRoles Role
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue(ClaimTypes.Role);

                if (!Enum.TryParse<TenantRoles>(role, out var parsedRole))
                    throw new UnauthorizedAccessException("User role claim is missing.");

                return parsedRole;
            }
        }

        public Guid TenantId
        {
            get
            {
                var tenantId = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue("tenant_id");

                if (!Guid.TryParse(tenantId, out var id))
                    throw new UnauthorizedAccessException(
                        "Tenant ID claim is missing.");

                return id;
            }
        }

        public bool IsPlatformAdmin
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?
                    .User.FindFirstValue(ClaimTypes.Role);

                return Enum.TryParse<PlatformRole>(role, out var parsedRole)
                    && parsedRole == PlatformRole.PlatformAdmin;
            }
        }
    }
}
