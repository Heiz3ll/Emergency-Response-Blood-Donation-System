using BloodDonorSystem.Domain.Common;

namespace BloodDonorSystem.Domain.Events;

public sealed record DonorRegisteredEvent(
    Guid DonorId,
    string Email,
    string BloodType) : DomainEvent;