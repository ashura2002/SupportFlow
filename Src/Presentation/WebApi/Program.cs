using Application;
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
app.UseRateLimiter();
app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseAuthorization();
app.MapControllers();

app.Run();
