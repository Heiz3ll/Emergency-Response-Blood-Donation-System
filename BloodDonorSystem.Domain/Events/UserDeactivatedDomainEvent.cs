using BloodDonorSystem.Domain.Common;

namespace BloodDonorSystem.Domain.Events;

public sealed record UserDeactivatedDomainEvent(
    Guid UserId)
    : DomainEvent;