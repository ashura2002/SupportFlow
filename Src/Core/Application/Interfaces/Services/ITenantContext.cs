namespace Application.Interfaces.Services
{
    public interface ITenantContext
    {
        string SchemaName { get; }
        bool IsResolved { get; }
        void SetTenant(string schemaName);
    }
}
