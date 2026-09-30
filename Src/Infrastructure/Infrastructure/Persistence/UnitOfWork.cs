using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Data;

namespace Infrastructure.Persistence
{
    internal class UnitOfWork : IUnitOfWork
    {
        private readonly SupportFlowDbContext _context;
        private readonly IEventDispatcher _eventDispatcher;

        public UnitOfWork(SupportFlowDbContext context, IEventDispatcher eventDispatcher)
        {
            _context = context;
            _eventDispatcher = eventDispatcher;
        }

        public async Task SaveChangesAsync(CancellationToken ct)
        {
            // every SaveChanges enters the loop and stops when there are no domain events
            while (true)
            {
                var domainEvents = CollectAndClearEvents();
                // stop processing when there are no domain events left
                if (domainEvents.Count == 0)
                {
                    break;
                }
                await _eventDispatcher.DispatchAsync(domainEvents, ct);
            }

            await _context.SaveChangesAsync(ct);
        }



        private List<IDomainEvent> CollectAndClearEvents()
        {
            var aggregates = _context.ChangeTracker
                .Entries<Aggregate>()
                .Select(entry => entry.Entity)
                .Where(entry => entry.DomainEvents.Count > 0)
                .ToList();

            var domainEvents = aggregates
                .SelectMany(aggregate => aggregate.DomainEvents)
                .ToList();

            foreach (var item in aggregates)
            {
                item.ClearEvents();
            }

            return domainEvents;
        }
    }
}
