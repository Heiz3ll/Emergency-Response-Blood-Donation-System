namespace BloodDonorSystem.Infrastructure.Persistence;

public sealed class DatabaseOptions
{
    public const string SectionName = "Database";

    public string ConnectionString { get; init; }
        = string.Empty;

    public int MaxRetryCount { get; init; } = 5;

    public int MaxRetryDelaySeconds { get; init; } = 30;

    public bool EnableSensitiveDataLogging { get; init; }

    public bool EnableDetailedErrors { get; init; }
}