using BloodDonorSystem.Application.Interfaces;
using BloodDonorSystem.Application.Messaging;
using BloodDonorSystem.Application.Services;
using BloodDonorSystem.Domain.Repository;
using BloodDonorSystem.Infrastructure.Identity;
using BloodDonorSystem.Infrastructure.Messaging;
using BloodDonorSystem.Infrastructure.Persistence;
using BloodDonorSystem.Infrastructure.Persistence.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BloodDonorSystem.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(
                configuration.GetConnectionString(
                    "DefaultConnection"));
        });

        services
            .AddIdentity<ApplicationUser, ApplicationRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddScoped<IUnitOfWork>(
            sp => sp.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<IUserRepository,
            UserRepository>();

        services.AddScoped<IIdentityService,
            IdentityService>();

        services.AddScoped<ICommandBus,
            CommandBus>();

        services.AddScoped(
            typeof(ICommandBehavior<>),
            typeof(TransactionBehavior<>));

        services.AddScoped(
            typeof(ICommandBehavior<>),
            typeof(LoggingBehavior<>));

        services.AddScoped(
            typeof(ICommandBehavior<,>),
            typeof(TransactionBehavior<,>));

        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c =>
                c.AssignableTo(typeof(ICommandHandler<>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        services.Scan(scan => scan
            .FromApplicationDependencies()
            .AddClasses(c =>
                c.AssignableTo(typeof(ICommandHandler<,>)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

        return services;
    }
}