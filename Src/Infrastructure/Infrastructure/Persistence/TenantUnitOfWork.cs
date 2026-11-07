

using Application.Interfaces.Services;
using Application.Interfaces.Uof;
using Domain.Entities;
using Domain.Events;
using Infrastructure.Data.DbContexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public sealed class TenantUnitOfWork : ITenantUnitOfWork
    {
        public readonly TenantDbContext _tenantDbContext;
        private readonly IEventDispatcher _eventDispatcher;

        public TenantUnitOfWork(TenantDbContext tenantDbContext, IEventDispatcher eventDispatcher)
        {
            _tenantDbContext = tenantDbContext;
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

            await _tenantDbContext.SaveChangesAsync(ct);
        }



        private List<IDomainEvent> CollectAndClearEvents()
        {
            var aggregates = _tenantDbContext.ChangeTracker
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
