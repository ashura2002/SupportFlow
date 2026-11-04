using Application.Common.Errors;
using Domain.Enums;

namespace WebApi.Common.Errors
{
    public static class ErrorResponseMapper
    {
        public static (int StatusCode, string Title) Map(Error error)
        {
            return error.Type switch
            {
                ErrorType.BadRequest => (StatusCodes.Status400BadRequest, "Bad Request"),
                ErrorType.Conflict => (StatusCodes.Status409Conflict, "Conflict"),
                ErrorType.Forbidden => (StatusCodes.Status403Forbidden, "Forbidden"),
                ErrorType.NotFound => (StatusCodes.Status404NotFound, "Not Found"),
                ErrorType.Unauthorized => (StatusCodes.Status401Unauthorized, "Unauthorized"),
                _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
            };
        }
    }
}
