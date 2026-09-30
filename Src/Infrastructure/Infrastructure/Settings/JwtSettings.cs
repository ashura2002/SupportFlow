
namespace Infrastructure.Settings
{
    public sealed class JwtSettings
    {
        public const string SectionName = "Jwt";
        public required string Key { get; set; }
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required int ExpiryInHours { get; set; }
    }
}