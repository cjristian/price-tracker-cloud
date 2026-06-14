using MediatR;
using PriceTrackerCloud.Application.DTOs.Products;

namespace PriceTrackerCloud.Application.Commands.Products;

public record CreateProductCommand(string Name, string Description, string Category) : IRequest<ProductDto>;
