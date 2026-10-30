
namespace Application.Interfaces.Services
{
    public interface ITenantProvisioningService
    {
        Task ProvisionAsync(string schemaName, CancellationToken cancellationToken);
    }
}
