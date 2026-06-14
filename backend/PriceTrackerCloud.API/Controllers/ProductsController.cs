using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PriceTrackerCloud.Application.Commands.Products;
using PriceTrackerCloud.Application.DTOs.Products;
using PriceTrackerCloud.Application.Queries.Products;

namespace PriceTrackerCloud.API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _sender.Send(new GetProductsQuery());
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _sender.Send(new GetProductByIdQuery(id));
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductDto dto)
    {
        var result = await _sender.Send(
            new CreateProductCommand(dto.Name, dto.Description, dto.Category));
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }
}
