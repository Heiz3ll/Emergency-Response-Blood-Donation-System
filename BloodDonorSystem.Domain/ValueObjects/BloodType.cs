using BloodDonorSystem.Domain.Common;

namespace BloodDonorSystem.Domain.ValueObjects;

public sealed class BloodType : ValueObject
{
    public AboGroup Group    { get; }
    public RhFactor RhFactor { get; }

    private BloodType(AboGroup group, RhFactor rhFactor)
    {
        Group    = group;
        RhFactor = rhFactor;
    }

    public static BloodType Create(AboGroup group, RhFactor rhFactor) =>
        new(group, rhFactor);

    public static BloodType Parse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Blood type cannot be empty.", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (!normalized.EndsWith('+') && !normalized.EndsWith('-'))
            throw new ArgumentException(
                $"'{value}' is not a valid blood type. Expected format: A+, O-, AB+, etc.");

        var aboRaw = normalized[..^1];
        var rhRaw  = normalized[^1..];

        var group = aboRaw switch
        {
            "A"  => AboGroup.A,
            "B"  => AboGroup.B,
            "AB" => AboGroup.AB,
            "O"  => AboGroup.O,
            _    => throw new ArgumentException($"'{aboRaw}' is not a valid ABO group."),
        };

        var rh = rhRaw switch
        {
            "+" => RhFactor.Positive,
            "-" => RhFactor.Negative,
            _   => throw new ArgumentException($"'{rhRaw}' is not a valid Rh factor."),
        };

        return new BloodType(group, rh);
    }

    public bool CanReceiveFrom(BloodType donor)
    {
        if (!IsRhCompatible(donor.RhFactor))
            return false;

        return (Group, donor.Group) switch
        {
            (AboGroup.AB, _)                         => true,
            (AboGroup.A,  AboGroup.A or AboGroup.O) => true,
            (AboGroup.B,  AboGroup.B or AboGroup.O) => true,
            (AboGroup.O,  AboGroup.O)               => true,
            _                                        => false,
        };
    }

    private bool IsRhCompatible(RhFactor donorRh) =>
        RhFactor == RhFactor.Positive || donorRh == RhFactor.Negative;

    public override string ToString() =>
        $"{Group}{(RhFactor == RhFactor.Positive ? "+" : "-")}";

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Group;
        yield return RhFactor;
    }
}