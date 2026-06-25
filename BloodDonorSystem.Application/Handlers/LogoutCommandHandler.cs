using BloodDonorSystem.Application.Command;
using BloodDonorSystem.Application.Messaging;
using BloodDonorSystem.Application.Services;

namespace BloodDonorSystem.Application.Handlers;

public sealed class LogoutCommandHandler
    : ICommandHandler<LogoutCommand>
{
    private readonly IIdentityService _identity;

    public LogoutCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public Task HandleAsync(
        LogoutCommand command,
        CancellationToken cancellationToken = default)
    {
        return _identity.LogoutAsync(
            command.RefreshToken,
            cancellationToken);
    }
}