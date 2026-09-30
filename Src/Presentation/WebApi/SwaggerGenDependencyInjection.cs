using Microsoft.OpenApi;

namespace WebAPI
{
    public static class SwaggerGenDependencyInjection
    {
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "SupportFlow",
                    Description = """
                        A RESTful API for managing support tickets, ticket conversations, categories, users, assignments, and support workflows. 
                        
                        Built with ASP.NET Core using Clean Architecture, CQRS, DDD, and SOLID principles 
                        to promote separation of concerns, maintainability, scalability, and clear domain boundaries.
                        """,
                    Version = "v1"
                });

                opt.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "JWT Authorization header using the Bearer scheme."
                });
                opt.AddSecurityRequirement(docs => new OpenApiSecurityRequirement
                {
                    [new OpenApiSecuritySchemeReference("bearer", docs)] = new()
                });
            }); ;


            return services;
        }
    }
}