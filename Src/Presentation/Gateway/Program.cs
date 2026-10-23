using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// for yarp reverse proxy
builder.Services
    .AddReverseProxy()
    .LoadFromConfig(
        builder.Configuration.GetSection("ReverseProxy"));


builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
    {
        var clientIpAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        Console.WriteLine($"[RateLimiter] Client IP: {clientIpAddress}");

        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: clientIpAddress,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100, // 100 requeste per limit
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                AutoReplenishment = true
            });
    });

    options.OnRejected = async (context, cancellationToken) =>
    {

        Console.WriteLine($"[RateLimiter] Request rejected. Client IP: " + $"{context.HttpContext.Connection.RemoteIpAddress}");


        context.HttpContext.Response.ContentType = "application/json";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                message = "Too many requests. Please try again later."
            },
            cancellationToken);
    };

});

var app = builder.Build();

app.UseRateLimiter();
app.MapReverseProxy();

app.Run();
