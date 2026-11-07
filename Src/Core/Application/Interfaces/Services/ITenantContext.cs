namespace Application.Interfaces.Services
{
    public interface ITenantContext
    {
        string SchemaName { get; }
        void SetTenant(string schemaName);
    }
}
