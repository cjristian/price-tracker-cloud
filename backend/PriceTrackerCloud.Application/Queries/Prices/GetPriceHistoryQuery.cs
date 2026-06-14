using MediatR;
using PriceTrackerCloud.Application.DTOs.Prices;

namespace PriceTrackerCloud.Application.Queries.Prices;

public record GetPriceHistoryQuery(Guid ProductId) : IRequest<IEnumerable<ProductPriceDto>>;
