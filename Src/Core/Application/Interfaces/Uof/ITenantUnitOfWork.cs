

namespace Application.Interfaces.Uof
{
    public interface ITenantUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken ct);
    }
}
