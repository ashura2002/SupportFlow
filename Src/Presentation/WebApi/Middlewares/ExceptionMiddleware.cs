using Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace WebAPI.Middlewares
{
    public sealed class ExceptionMiddleware : IExceptionHandler
    {
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(
            ILogger<ExceptionMiddleware> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {

            if (exception is DomainRuleViolationException
                or ValidationException
                or DbUpdateConcurrencyException)
            {
                _logger.LogWarning(exception, "Request failed.");
            }
            else
            {
                // log unexpected exceptions as errors for investigation
                _logger.LogError(exception, "Unhandled exception occurred");
            }


            var statusCode = exception switch
            {
                DomainRuleViolationException => StatusCodes.Status400BadRequest,
                ValidationException => StatusCodes.Status400BadRequest, // para sa fluent validationnnnnnnn
                DbUpdateConcurrencyException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            var fluentValidationErrors = exception is ValidationException errors
                ? errors.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(err =>
                    err.ErrorMessage).ToList()) : null;

            // for concurrency exception message
            var message = exception is DbUpdateConcurrencyException ?
                "The resource was modified by another request. Please try again." : exception.Message;

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Detail = statusCode == StatusCodes.Status500InternalServerError ?
                "An unexpected error occurred."
                : message
            };

            // only add errors extension when exception came from fluent validation
            if (fluentValidationErrors is not null)
            {
                problemDetails.Extensions["errors"] = fluentValidationErrors;
            }

            httpContext.Response.StatusCode = statusCode;
            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
            return true;
        }
    }
}