using Application.Interfaces.Services;

namespace Infrastructure.Services
{
    public sealed class TenantContext : ITenantContext
    {
        public string SchemaName { get; private set; } = string.Empty;

        public bool IsResolved => !string.IsNullOrWhiteSpace(SchemaName);

        public void SetTenant(string schemaName)
        {
            if (string.IsNullOrWhiteSpace(schemaName))
                throw new ArgumentException("Schema name cannot be empty.", nameof(schemaName));
            SchemaName = schemaName;
        }
    }
}
