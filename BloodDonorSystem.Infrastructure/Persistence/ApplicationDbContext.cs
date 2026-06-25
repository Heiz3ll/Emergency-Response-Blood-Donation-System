using BloodDonorSystem.Application.Interfaces;
using BloodDonorSystem.Domain.Entities;
using BloodDonorSystem.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodDonorSystem.Infrastructure.Persistence;

public sealed class ApplicationDbContext
    : IdentityDbContext<
            ApplicationUser,
            ApplicationRole,
            Guid>,
        IUnitOfWork
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();

    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(
        ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}