using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Commands.Auth;
using PriceTrackerCloud.Application.DTOs.Auth;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AuthController : ControllerBase
{
    private readonly ISender _sender;

    public AuthController(ISender sender)
    {
        _sender = sender;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _sender.Send(
            new RegisterUserCommand(dto.Name, dto.Email, dto.Password));
        return CreatedAtAction(nameof(Register), result);
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var result = await _sender.Send(
            new LoginUserCommand(dto.Email, dto.Password));
        return Ok(result);
    }
}
