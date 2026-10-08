using Application;
using Domain.Exceptions;
using FluentValidation;
using Infrastructure;
using Infrastructure.Data;
using Serilog;
using WebAPI;
using WebAPI.Middlewares;


var builder = WebApplication.CreateBuilder(args);

// logging
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File(
        "logs/SupportFlow-.log",
        rollingInterval: RollingInterval.Day)
    .CreateLogger();
builder.Host.UseSerilog();

// sentry
builder.WebHost.UseSentry(option =>
{
    option.Dsn = builder.Configuration["Sentry:Dsn"];


    // exceptions that are expected and should not create Sentry issues
    var ignoreExpectedExceptions = new[]
    {
        typeof(DomainRuleViolationException),
        typeof(ValidationException)
    };


    // filter captured Sentry events before they are sent
    option.SetBeforeSend((sentryEvent, hint) =>
    {
        var exeption = sentryEvent.Exception;

        if (exeption is not null && ignoreExpectedExceptions.Contains(exeption.GetType()))
            return null;


        return sentryEvent;
    }); 
});

// healthcheck for postgresql
builder.Services
    .AddHealthChecks()
    .AddDbContextCheck<SupportFlowDbContext>();

builder.Services.AddCors(option => option.AddPolicy("AllowAll", policy =>
{
    policy.AllowAnyOrigin()
          .AllowAnyHeader()
          .AllowAnyMethod();
}));

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddJwtAuthenticationDI(builder.Configuration);
builder.Services.AddSwaggerDocumentation();



builder.Services.AddRateLimiting();

// error handling middleware with problem details
builder.Services.AddExceptionHandler<ExceptionMiddleware>();
builder.Services.AddProblemDetails();

var app = builder.Build();

// seeded data
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAdminUser();
}

// for Swaggger UI
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/db-health");
app.MapControllers();

app.Run();
