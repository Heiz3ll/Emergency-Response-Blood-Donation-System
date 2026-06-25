using BloodDonorSystem.Domain.Common;

namespace BloodDonorSystem.Domain.Events;

public sealed record UserRegisteredDomainEvent(
    Guid UserId,
    string Email)
    : DomainEvent;