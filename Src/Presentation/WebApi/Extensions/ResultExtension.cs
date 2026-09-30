using Application.Common.Errors;
using Application.Common.Results;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc;

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


        public static ActionResult ToErrorActionResult(this ControllerBase controller, Error error)
        {
            return error.Type switch
            {
                ErrorType.BadRequest => controller.BadRequest(error),
                ErrorType.Conflict => controller.Conflict(error),
                ErrorType.NotFound => controller.NotFound(error),
                ErrorType.Unauthorized => controller.Unauthorized(error),
                _ => controller.StatusCode(500)
            };
        }

    }
}
