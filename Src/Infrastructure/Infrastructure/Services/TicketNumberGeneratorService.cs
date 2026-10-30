using Application.Interfaces.Services;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Services
{
    public sealed class TicketNumberGeneratorService : ITicketNumberGeneratorService
    {
        private readonly SharedDbContext _context;

        public TicketNumberGeneratorService(
            SharedDbContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateTicketNumber(CancellationToken ct)
        {
            var ticketNumber = await _context.Database
                // use a postgreSQL sequence to generate a concurrency safe unique number
                .SqlQuery<long>($"SELECT nextval('ticket_number_seq') AS \"Value\"")
                .SingleAsync(ct);
            return $"SF-{ticketNumber:D6}";
        }
    }
}
