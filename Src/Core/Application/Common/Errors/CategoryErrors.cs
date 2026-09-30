
using Domain.Enums;

namespace Application.Common.Errors
{
    public static class CategoryErrors
    {
        public static readonly Error CategoryNotFound = new(
            "Category.NotFound",
            "Category not found.",
            ErrorType.NotFound);

        public static readonly Error CategoryNameExist = new(
            "Category.Conflict",
            "Category name already existed.",
            ErrorType.Conflict);
    }
}
