using BloodDonorSystem.Domain.Aggregates;

namespace BloodDonorSystem.Domain.Common;

public abstract class AuditableAggregateRoot : AggregateRoot
{
    public DateTime CreatedAtUtc { get; protected set; }

    public DateTime? UpdatedAtUtc { get; protected set; }

    protected AuditableAggregateRoot()
    {
    }

    protected AuditableAggregateRoot(Guid id)
        : base(id)
    {
        CreatedAtUtc = DateTime.UtcNow;
    }

    protected void SetUpdated()
    {
        UpdatedAtUtc = DateTime.UtcNow;
    }
}