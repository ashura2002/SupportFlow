using Domain.Enums;

namespace Application.ResponseDTO
{
    public sealed record UserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        Roles Role);
}
