
using Domain.Enums;

namespace Application.Common.Errors
{
    public static class TenantErrors
    {
        public static readonly Error TenantNotFound = new(
            "Tenant.NotFound",
            "Tenant not found.",
            ErrorType.NotFound);

        public static readonly Error TenantIdentifierAlreadyExists = new(
            "Tenant.TenantIdentifierAlreadyExisted",
            "Tenant identifier is already existed.",
            ErrorType.Conflict);

        public static readonly Error SchemaAlreadyExists = new(
          "Tenant.SchemaAlreayExisted",
          "Schema is already existed.",
          ErrorType.Conflict);
    }
}
