using BloodDonorSystem.Application.DTOs;

namespace BloodDonorSystem.Application.Services;

public interface IIdentityService
{
    Task<AuthDto> CreateUserAsync(
        Guid userId,
        string email,
        string password,
        string role,
        CancellationToken cancellationToken = default);

    Task<AuthDto> LoginAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default);

    Task<AuthDto> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);

    Task LogoutAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}