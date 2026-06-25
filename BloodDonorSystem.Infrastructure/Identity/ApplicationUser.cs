using Microsoft.AspNetCore.Identity;

namespace BloodDonorSystem.Infrastructure.Identity;

public sealed class ApplicationUser
    : IdentityUser<Guid>
{
    public Guid DomainUserId { get; set; }

    public string FirstName { get; set; }
        = string.Empty;

    public string LastName { get; set; }
        = string.Empty;

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }

    public DateTime? UpdatedAtUtc { get; set; }
}