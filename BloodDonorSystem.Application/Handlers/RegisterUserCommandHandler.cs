using BloodDonorSystem.Application.Command;
using BloodDonorSystem.Application.DTOs;
using BloodDonorSystem.Application.Messaging;
using BloodDonorSystem.Application.Services;

namespace BloodDonorSystem.Application.Handlers;

public sealed class RegisterUserCommandHandler
    : ICommandHandler<RegisterUserCommand, AuthDto>
{
    private readonly IIdentityService _identity;

    public RegisterUserCommandHandler(IIdentityService identity)
    {
        _identity = identity;
    }

    public async Task<AuthDto> HandleAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        Guid userId = Guid.CreateVersion7();

        return await _identity.CreateUserAsync(
            userId,
            command.Email,
            command.Password,
            command.Role,
            cancellationToken);

    }
}