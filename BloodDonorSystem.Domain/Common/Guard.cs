namespace BloodDonorSystem.Domain.Common;

public static class Guard
{
    public static void AgainstNull(
        object? value,
        string parameterName)
    {
        if (value is null)
            throw new DomainException(
                $"{parameterName} cannot be null.");
    }

    public static void AgainstNullOrWhiteSpace(
        string? value,
        string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException(
                $"{parameterName} cannot be empty.");
    }
}