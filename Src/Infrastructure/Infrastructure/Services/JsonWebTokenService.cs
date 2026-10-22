using Application.Interfaces.Services;
using Domain.Entities;
using Infrastructure.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Domain.Enums;

namespace Infrastructure.Services
{
    internal sealed class JsonWebTokenService : IJwtService
    {
        private readonly JwtSettings _jwtSettings;

        public JsonWebTokenService(IOptions<JwtSettings> options)
        {
            _jwtSettings = options.Value;
        }

        public string GenerateAccessToken(
            User user,
            Guid tenantId,
            TenantRoles roles)
        {
            List<Claim> claims = new()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new("tenant_id", tenantId.ToString()),
                new(ClaimTypes.Role, roles.ToString()),
                new(ClaimTypes.Email, user.Email.Value.ToString())
            };

            // create security key - get key from the app setting or user secrets
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key)) ??
                throw new Exception("JWT Key is missing. Check appsettings configuration or in user-secrets.");

            // sign credentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


            // actual Jwt is created
            var accessToken = new JwtSecurityToken(issuer: _jwtSettings.Issuer,
                            audience: _jwtSettings.Audience,
                            claims: claims,
                            expires: DateTime.UtcNow.AddHours(_jwtSettings.ExpiryInHours),
                            signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(accessToken);
        }

        public string GeneratePlatformAccessToken(User user){


            List<Claim> claims = new()
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Role, PlatformRole.PlatformAdmin.ToString()),
                new(ClaimTypes.Email, user.Email.Value.ToString())
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_jwtSettings.Key));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(
                    _jwtSettings.ExpiryInHours),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
