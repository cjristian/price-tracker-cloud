using AutoMapper;
using MediatR;
using PriceTrackerCloud.Application.DTOs.Prices;
using PriceTrackerCloud.Application.Interfaces;

namespace PriceTrackerCloud.Application.Queries.Prices;

public class GetPriceHistoryQueryHandler : IRequestHandler<GetPriceHistoryQuery, IEnumerable<ProductPriceDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetPriceHistoryQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductPriceDto>> Handle(GetPriceHistoryQuery request, CancellationToken cancellationToken)
    {
        var prices = await _uow.Prices.GetHistoryByProductAsync(request.ProductId);
        return _mapper.Map<IEnumerable<ProductPriceDto>>(prices);
    }
}
