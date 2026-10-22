using Application.Interfaces.Services;

namespace Infrastructure.Services
{
    public sealed class SchemaNameGenerator : ISchemaNameGenerator
    {
        public string Generate(string identifier)
        {
            return $"tenant_{identifier.Replace("-", "_").ToLowerInvariant()}";
        }
    }
}
