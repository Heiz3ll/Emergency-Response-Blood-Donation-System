using BloodDonorSystem.Application.DTOs;
using BloodDonorSystem.Application.Messaging;

namespace BloodDonorSystem.Application.Command;

public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string Role) : ICommand<AuthDto>;