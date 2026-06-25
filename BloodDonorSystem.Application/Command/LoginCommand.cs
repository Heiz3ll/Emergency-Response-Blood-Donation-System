using BloodDonorSystem.Application.DTOs;
using BloodDonorSystem.Application.Messaging;

namespace BloodDonorSystem.Application.Command;

public sealed record LoginCommand(
    string Email,
    string Password) : ICommand<AuthDto>;