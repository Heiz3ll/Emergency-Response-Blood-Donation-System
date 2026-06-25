namespace BloodDonorSystem.Domain.Common;

public abstract record DomainEvent
{
    public DateTime OccurredOnUtc { get; } = DateTime.UtcNow;
}