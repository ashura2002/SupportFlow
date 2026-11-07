
namespace Application.Interfaces.Uof
{
    public interface ISharedUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken ct);
    }
}
