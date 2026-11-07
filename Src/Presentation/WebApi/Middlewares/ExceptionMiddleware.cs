using Domain.Exceptions;
using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApi.Common.Errors;

namespace WebApi.Middlewares;

public sealed class ExceptionMiddleware(
    ILogger<ExceptionMiddleware> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail, errorCode) = exception switch
        {
            ValidationException =>
                (
                    StatusCodes.Status400BadRequest,
                    "Validation Failed",
                    "One or more validation errors occurred.",
                    ApiErrorCodes.ValidationError
                ),

            DomainRuleViolationException =>
                (
                    StatusCodes.Status400BadRequest,
                    "Bad Request",
                    exception.Message,
                    ApiErrorCodes.DomainRuleViolation
                ),

            DbUpdateConcurrencyException =>
                (
                    StatusCodes.Status409Conflict,
                    "Conflict",
                    "The resource was modified by another request. Please try again.",
                    ApiErrorCodes.ConcurrencyConflict
                ),

            _ =>
                (
                    StatusCodes.Status500InternalServerError,
                    "Internal Server Error",
                    "An unexpected error occurred.",
                    ApiErrorCodes.InternalServerError
                )
        };

        if (statusCode >= 500)
            logger.LogError(exception, "Unhandled exception occurred.");
        else
            logger.LogWarning(exception, "Request failed.");

        ProblemDetails problemDetails;

        if (exception is ValidationException validationException)
        {
            var errors = validationException.Errors
                .GroupBy(error => error.PropertyName)
                .ToDictionary(
                    group => group.Key,
                    group => group.Select(error => 
                    error.ErrorMessage).ToArray());

            problemDetails = new ValidationProblemDetails(errors)
            {
                Status = statusCode,
                Title = title,
                Detail = detail,
                Instance = httpContext.Request.Path
            };

            problemDetails.Extensions["traceId"] =
                httpContext.TraceIdentifier;

            problemDetails.Extensions["errorCode"] = errorCode;
        }
        else
        {
            problemDetails = ProblemDetailsFactory.Create(
                httpContext,
                statusCode,
                title,
                detail,
                errorCode);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}