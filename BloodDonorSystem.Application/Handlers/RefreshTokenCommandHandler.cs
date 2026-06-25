using BloodDonorSystem.Application.Command;
using BloodDonorSystem.Application.DTOs;
using BloodDonorSystem.Application.Messaging;
using BloodDonorSystem.Application.Services;

namespace BloodDonorSystem.Application.Handlers;

public sealed class RefreshTokenCommandHandler
    : ICommandHandler<RefreshTokenCommand, AuthDto>
{
    private readonly IIdentityService _identity;

    public RefreshTokenCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public Task<AuthDto> HandleAsync(
        RefreshTokenCommand command,
        CancellationToken cancellationToken = default)
    {
        return _identity.RefreshTokenAsync(
            command.RefreshToken,
            cancellationToken);
    }
}