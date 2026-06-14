using AutoMapper;
using MediatR;
using PriceTrackerCloud.Application.DTOs.Products;
using PriceTrackerCloud.Application.Interfaces;

namespace PriceTrackerCloud.Application.Queries.Products;

public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProductsQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _uow.Products.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
