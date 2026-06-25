using BloodDonorSystem.Infrastructure.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BloodDonorSystem.Infrastructure.Persistence.Configurations;

public sealed class ApplicationRoleConfiguration
    : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(
        EntityTypeBuilder<ApplicationRole> builder)
    {
        builder.ToTable("AspNetRoles");
    }
}