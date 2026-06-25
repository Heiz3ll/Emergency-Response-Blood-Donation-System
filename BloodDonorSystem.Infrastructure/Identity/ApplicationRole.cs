using Microsoft.AspNetCore.Identity;

namespace BloodDonorSystem.Infrastructure.Identity;

public sealed class ApplicationRole
    : IdentityRole<Guid>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName)
        : base(roleName)
    {
        Id = Guid.CreateVersion7();
    }
}