using BloodDonorSystem.Domain.Common;

namespace BloodDonorSystem.Domain.Aggregates;

public abstract class AggregateRoot : Entity
{
    private readonly List<DomainEvent> _domainEvents = [];

    public IReadOnlyCollection<DomainEvent> DomainEvents =>
        _domainEvents.AsReadOnly();

    public int Version { get; private set; }

    protected AggregateRoot()
    {
    }

    protected AggregateRoot(Guid id)
        : base(id)
    {
    }

    protected void RaiseDomainEvent(DomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public void ClearDomainEvents()
    {
        _domainEvents.Clear();
    }

    protected void IncrementVersion()
    {
        Version++;
    }
}