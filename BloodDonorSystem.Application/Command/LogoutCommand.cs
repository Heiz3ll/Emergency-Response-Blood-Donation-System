using BloodDonorSystem.Application.Messaging;

namespace BloodDonorSystem.Application.Command;

public sealed record LogoutCommand(
    string RefreshToken) : ICommand;