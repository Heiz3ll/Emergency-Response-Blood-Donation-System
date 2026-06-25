using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BloodDonorSystem.Infrastructure.Persistence;

public static class DatabaseServiceCollectionExtensions
{
    public static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        DatabaseOptions options =
            configuration
                .GetSection(DatabaseOptions.SectionName)
                .Get<DatabaseOptions>()
            ?? throw new InvalidOperationException(
                "Database configuration missing.");

        services.AddDbContext<ApplicationDbContext>(db =>
        {
            db.UseNpgsql(
                options.ConnectionString,
                npgsql =>
                {
                    npgsql.EnableRetryOnFailure(
                        options.MaxRetryCount,
                        TimeSpan.FromSeconds(
                            options.MaxRetryDelaySeconds),
                        null);
                });

            if (options.EnableSensitiveDataLogging)
            {
                db.EnableSensitiveDataLogging();
            }

            if (options.EnableDetailedErrors)
            {
                db.EnableDetailedErrors();
            }
        });

        return services;
    }
}