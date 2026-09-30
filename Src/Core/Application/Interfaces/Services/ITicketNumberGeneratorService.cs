
namespace Application.Interfaces.Services
{
    public interface ITicketNumberGeneratorService
    {
        Task<string> GenerateTicketNumber(CancellationToken ct);
    }
}
