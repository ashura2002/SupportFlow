
namespace Application.Interfaces.Uof
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync(CancellationToken ct);
    }
}
