using AutoMapper;
using MediatR;
using PriceTrackerCloud.Application.DTOs.Alerts;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Commands.Alerts;

public class CreateAlertCommandHandler : IRequestHandler<CreateAlertCommand, AlertDto>
{
    private readonly IUnitOfWork _uow;
    private readonly IMapper _mapper;

    public CreateAlertCommandHandler(IUnitOfWork uow, IMapper mapper)
    {
        _uow = uow;
        _mapper = mapper;
    }

    public async Task<AlertDto> Handle(CreateAlertCommand request, CancellationToken cancellationToken)
    {
        var productExists = await _uow.Products.GetByIdAsync(request.ProductId);
        if (productExists is null)
            throw new KeyNotFoundException($"Producto con Id '{request.ProductId}' no encontrado.");

        var alert = new Alert
        {
            UserId      = request.UserId,
            ProductId   = request.ProductId,
            TargetPrice = request.TargetPrice
        };

        await _uow.Alerts.AddAsync(alert);
        await _uow.SaveChangesAsync(cancellationToken);

        // Recargamos con el producto incluido para el DTO
        alert.Product = productExists;
        return _mapper.Map<AlertDto>(alert);
    }
}
