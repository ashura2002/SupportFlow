
using Domain.Enums;

namespace Application.Common.Errors
{
    public static class UserErrors
    {
        public static readonly Error EmailExist = new(
            "User.EmailConflict",
            "Email already exist.",
            ErrorType.Conflict);

        public static readonly Error UserNotFound = new(
            "User.NotFound",
            "User not found",
            ErrorType.NotFound);

        public static readonly Error UserRoleNotAgent = new(
              "User.RoleNotAgent",
              "User is not a support agent.",
              ErrorType.BadRequest);
    }
}
