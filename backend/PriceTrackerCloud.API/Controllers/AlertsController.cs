using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Commands.Alerts;
using PriceTrackerCloud.Application.DTOs.Alerts;
using PriceTrackerCloud.Application.Queries.Alerts;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AlertsController : ControllerBase
{
    private readonly ISender _sender;

    public AlertsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _sender.Send(new GetUserAlertsQuery(userId));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAlertDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var result = await _sender.Send(
            new CreateAlertCommand(userId, dto.ProductId, dto.TargetPrice));
        return CreatedAtAction(nameof(GetAll), result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await _sender.Send(new DeleteAlertCommand(id, userId));
        return NoContent();
    }
}
