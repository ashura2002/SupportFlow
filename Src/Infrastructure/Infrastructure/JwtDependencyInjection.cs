using Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace Infrastructure
{
    public static class JwtDependencyInjection
    {

        public static IServiceCollection AddJwtAuthenticationDI(this IServiceCollection services, IConfiguration configuration)
        {

            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT settings are missing.");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                  .AddJwtBearer(options =>
                  {
                      options.TokenValidationParameters = new TokenValidationParameters
                      {
                          ValidateIssuer = true,
                          ValidIssuer = jwtSettings.Issuer,

                          ValidateAudience = true,
                          ValidAudience = jwtSettings.Audience,

                          ValidateLifetime = true,

                          ValidateIssuerSigningKey = true,
                          IssuerSigningKey =
                            new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(jwtSettings.Key))
                      };

                      // for error handling in forbidden
                      options.Events = new JwtBearerEvents
                      {
                          OnChallenge = context =>
                          {
                              context.HandleResponse();

                              context.Response.StatusCode = 401;
                              return context.Response.WriteAsJsonAsync(new
                              {
                                  message = "You are not authenticated"
                              });
                          },

                          OnForbidden = context =>
                          {
                              context.Response.StatusCode = 403;
                              return context.Response.WriteAsJsonAsync(new
                              {
                                  message = "Access denied (role mismatch)"
                              });
                          },

                          OnTokenValidated = async context =>
                          {
                              var userClaim = context.Principal?.FindFirst(ClaimTypes.NameIdentifier);

                              if (userClaim is null)
                              {
                                  context.Fail("User ID is missing.");
                                  return;
                              }

                              if (!Guid.TryParse(userClaim.Value, out var userId))
                              {
                                  context.Fail("Invalid User ID.");
                                  return;
                              }
                          }
                      };
                  });

            return services;
        }

    }
}