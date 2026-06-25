using BloodDonorSystem.Domain.Common;
using BloodDonorSystem.Domain.Events;
using BloodDonorSystem.Domain.ValueObjects;

namespace BloodDonorSystem.Domain.Entities;

public sealed class User : AuditableAggregateRoot
{
    private User()
    {
    }

    private User(
        Guid id,
        Email email,
        string firstName,
        string lastName)
        : base(id)
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        IsActive = true;

        RaiseDomainEvent(
            new UserRegisteredDomainEvent(
                id,
                email.Value));
    }

    public Email Email { get; private set; } = default!;

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public bool IsActive { get; private set; }

    public static User Create(
        Email email,
        string firstName,
        string lastName)
    {
        Guard.AgainstNullOrWhiteSpace(
            firstName,
            nameof(firstName));

        Guard.AgainstNullOrWhiteSpace(
            lastName,
            nameof(lastName));

        return new User(
            Guid.CreateVersion7(),
            email,
            firstName.Trim(),
            lastName.Trim());
    }

    public void UpdateName(
        string firstName,
        string lastName)
    {
        Guard.AgainstNullOrWhiteSpace(
            firstName,
            nameof(firstName));

        Guard.AgainstNullOrWhiteSpace(
            lastName,
            nameof(lastName));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();

        SetUpdated();
        IncrementVersion();
    }

    public void Activate()
    {
        if (IsActive)
            return;

        IsActive = true;

        SetUpdated();
        IncrementVersion();
    }

    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;

        RaiseDomainEvent(
            new UserDeactivatedDomainEvent(Id));

        SetUpdated();
        IncrementVersion();
    }
}