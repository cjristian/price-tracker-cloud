using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Queries.Prices;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class PricesController : ControllerBase
{
    private readonly ISender _sender;

    public PricesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet("history/{productId:guid}")]
    public async Task<IActionResult> GetHistory(Guid productId)
    {
        var result = await _sender.Send(new GetPriceHistoryQuery(productId));
        return Ok(result);
    }
}
