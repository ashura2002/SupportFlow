using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using WebApi.Attributes;

namespace WebApi.Middlewares
{
    public sealed class TenantResolutionMiddleware : IMiddleware
    {
        private readonly ITenantReadRepository _tenantReadRepository;
        private readonly ITenantContext _tenantContext;
        private readonly ILogger<TenantResolutionMiddleware> _logger;

        public TenantResolutionMiddleware(
            ITenantReadRepository tenantReadRepository,
            ITenantContext tenantContext,
            ILogger<TenantResolutionMiddleware> logger)
        {
            _tenantReadRepository = tenantReadRepository;
            _tenantContext = tenantContext;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var endpoint = context.GetEndpoint();
            var requiresTenant = endpoint?
                .Metadata
                .GetMetadata<RequireTenantAttribute>() is not null;


            if (!requiresTenant)
            {
                await next(context);
                return;
            }

            var identifier = context.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            _logger.LogInformation("Tenant Identifier - {Identifier}", identifier);

            if (string.IsNullOrWhiteSpace(identifier))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsync("Missing X-Tenant-id header.");
                return;
            }

    
            var tenant = await _tenantReadRepository.GetActiveTenantByIdentifierAsync(identifier, context.RequestAborted);
            if (tenant is null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsync("Tenant not found or inactive.");
                return;
            }

            _tenantContext.SetTenant(tenant.SchemaName);
            await next(context);
        }
    }
}