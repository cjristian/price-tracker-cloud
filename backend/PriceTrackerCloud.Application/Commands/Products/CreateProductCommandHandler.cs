using AutoMapper;
using MediatR;
using PriceTrackerCloud.Application.DTOs.Products;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Commands.Products;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateProductCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name        = request.Name,
            Description = request.Description,
            Category    = request.Category
        };

        await _uow.Products.AddAsync(product);
        await _uow.SaveChangesAsync(cancellationToken);

        return _mapper.Map<ProductDto>(product);
    }
}
