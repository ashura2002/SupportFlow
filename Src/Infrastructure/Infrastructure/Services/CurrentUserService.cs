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
                var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!Guid.TryParse(userId, out var id))
                    throw new UnauthorizedAccessException("User ID claim is missing.");

                return id;
            }
        }

        public Roles Role
        {
            get
            {
                var role = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
                if (!Enum.TryParse<Roles>(role, out var parsedRole))
                    throw new UnauthorizedAccessException("User role claim is missing.");

                return parsedRole;
            }
        }
    }
}
