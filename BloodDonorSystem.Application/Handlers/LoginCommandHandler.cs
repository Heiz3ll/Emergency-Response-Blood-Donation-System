using BloodDonorSystem.Application.Command;
using BloodDonorSystem.Application.DTOs;
using BloodDonorSystem.Application.Messaging;
using BloodDonorSystem.Application.Services;

namespace BloodDonorSystem.Application.Handlers;

public sealed class LoginCommandHandler
    : ICommandHandler<LoginCommand, AuthDto>
{
    private readonly IIdentityService _identity;

    public LoginCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public Task<AuthDto> HandleAsync(
        LoginCommand command,
        CancellationToken cancellationToken = default)
    {
        return _identity.LoginAsync(
            command.Email,
            command.Password,
            cancellationToken);
    }
}