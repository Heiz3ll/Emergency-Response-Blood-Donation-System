namespace BloodDonorSystem.Application.DTOs;
//Used by
//Login
//RefreshToken
public sealed class AuthDto
{
    public string AccessToken { get; init; } = string.Empty;

    public string RefreshToken { get; init; } = string.Empty;

    public DateTime ExpiresAtUtc { get; init; }

    public Guid UserId { get; init; }

    public string Email { get; init; } = string.Empty;

    public IReadOnlyCollection<string> Roles { get; init; }
        = [];
}