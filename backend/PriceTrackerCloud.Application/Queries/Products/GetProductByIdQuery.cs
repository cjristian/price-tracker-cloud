using MediatR;
using PriceTrackerCloud.Application.DTOs.Products;

namespace PriceTrackerCloud.Application.Queries.Products;

public record GetProductByIdQuery(Guid Id) : IRequest<ProductDto>;
