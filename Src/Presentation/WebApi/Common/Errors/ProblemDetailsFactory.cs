using Microsoft.AspNetCore.Mvc;

namespace WebApi.Common.Errors
{
    public static class ProblemDetailsFactory
    {
        public static ProblemDetails Create(
            HttpContext httpContext,
            int statusCode,
            string title,
            string detail,
            string? errorCode = null,
            IDictionary<string, string[]>? errors = null)
        {
            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

            if (errorCode is not null)
                problemDetails.Extensions["errorCode"] = errorCode;

            if (errors is not null)
                problemDetails.Extensions["errors"] = errors;

            return problemDetails;
        }
    }
}
