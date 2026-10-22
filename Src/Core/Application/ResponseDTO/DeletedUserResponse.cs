using Domain.Enums;

namespace Application.ResponseDTO;

public sealed record DeletedUserResponse(
        Guid Id,
        string FirstName,
        string LastName,
        string Email,
        TenantRoles? Role,
        string? TenantName,
        DateTime? DeletedAt);
