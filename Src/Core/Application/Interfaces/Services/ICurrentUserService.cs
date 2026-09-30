using Domain.Enums;

namespace Application.Interfaces.Services
{
    public interface ICurrentUserService
    {
        Guid UserId { get; }
        Roles Role { get; }
    }
}
