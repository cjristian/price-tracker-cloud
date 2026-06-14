using AutoMapper;
using MediatR;
using PriceTrackerCloud.Application.DTOs.Products;
using PriceTrackerCloud.Application.Interfaces;

namespace PriceTrackerCloud.Application.Queries.Products;

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public GetProductByIdQueryHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _uow.Products.GetByIdAsync(request.Id)
            ?? throw new KeyNotFoundException($"Producto '{request.Id}' no encontrado.");

        return _mapper.Map<ProductDto>(product);
    }
}
