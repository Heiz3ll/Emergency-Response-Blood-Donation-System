using BloodDonorSystem.Application.Command;
using BloodDonorSystem.Application.Messaging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly ICommandBus _commandBus;

    public AuthController(ICommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterUserCommand command)
    {
        var result = await _commandBus.ExecuteAsync(command);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginCommand command)
    {
        var result = await _commandBus.ExecuteAsync(command);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenCommand command)
    {
        var result = await _commandBus.ExecuteAsync(command);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(LogoutCommand command)
    {
        await _commandBus.ExecuteAsync(command);
        return Ok();
    }
}