using Domain.Events;

namespace Domain.Entities
{
    public abstract class Aggregate : BaseEntity
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly(); 

        protected void AddEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }

        public void ClearEvents()
        {
            Console.WriteLine("Event Clear.");
            _domainEvents.Clear();
        }
    }
}
