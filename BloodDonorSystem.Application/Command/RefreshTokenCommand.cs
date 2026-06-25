using BloodDonorSystem.Application.DTOs;
using BloodDonorSystem.Application.Messaging;

namespace BloodDonorSystem.Application.Command;

public sealed record RefreshTokenCommand(
    string RefreshToken) : ICommand<AuthDto>;