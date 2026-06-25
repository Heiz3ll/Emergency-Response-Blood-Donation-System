namespace BloodDonorSystem.Application.Exceptions;

public sealed class ValidationException : Exception
{
    public IReadOnlyCollection<string> Errors { get; }

    public ValidationException(
        IEnumerable<string> errors)
        : base("Validation failed.")
    {
        Errors = errors.ToList().AsReadOnly();
    }

    public ValidationException(string error)
        : this([error])
    {
    }
}