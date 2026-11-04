using Application.Common.Errors;
using Application.Common.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using WebApi.Common.Errors;

namespace WebApi.Extensions
{
    public static class ResultExtension
    {

        public static ActionResult<T> ToActionResult<T>(this ControllerBase controller, Result<T> result)
        {
            if (result.IsSuccess)
                return controller.Ok(result.Value);

            return controller.ToErrorActionResult(result.Error!);
        }

        public static ActionResult ToActionResult(this ControllerBase controller, Result result)
        {
            if (result.IsSuccess)
                return controller.NoContent();

            return controller.ToErrorActionResult(result.Error!);
        }


        private static ObjectResult ToErrorActionResult(this ControllerBase controller, Error error)
        {
            var (statusCode, title) = ErrorResponseMapper.Map(error);

            var problemDetails = ProblemDetailsFactory.Create(
                controller.HttpContext,
                statusCode,
                title,
                statusCode >= 500
                    ? "An unexpected error occured."
                    : error.Message,
                statusCode >= 500 
                    ? ApiErrorCodes.InternalServerError
                    : error.Code);

            return new ObjectResult(problemDetails)
            {
                StatusCode = statusCode,
                ContentTypes = { "application/problem+json" }
            };
        }

    }
}
