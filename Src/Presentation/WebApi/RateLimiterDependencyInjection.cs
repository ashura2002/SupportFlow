using Microsoft.AspNetCore.RateLimiting;
using System.Text.Json;

namespace WebAPI
{
    public static class RateLimiterDependencyInjection
    {
        public static IServiceCollection AddRateLimiting(this IServiceCollection services)
        {

            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                // to show the error msg in response
                options.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.ContentType = "application/json";

                    var response = new
                    {
                        StatusCode = StatusCodes.Status429TooManyRequests,
                        Message = "Too many requests. Please try again after 1 minute.",
                        TraceId = context.HttpContext.TraceIdentifier
                    };

                    await context.HttpContext.Response.WriteAsync(
                    JsonSerializer.Serialize(response), cancellationToken);
                };


                options.AddFixedWindowLimiter("LoginPolicy", opt =>
                {
                    opt.PermitLimit = 5;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });

                options.AddFixedWindowLimiter("GetResourcesPolicy", opt => {
                    opt.PermitLimit = 60;
                    opt.Window = TimeSpan.FromMinutes(1);
                    opt.QueueLimit = 0;
                });
            });


            return services;
        }
    }
}