using AutoMapper;
using PriceTrackerCloud.Application.DTOs.Alerts;
using PriceTrackerCloud.Application.DTOs.Prices;
using PriceTrackerCloud.Application.DTOs.Products;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>();

        CreateMap<ProductPrice, ProductPriceDto>()
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name));

        CreateMap<Alert, AlertDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
    }
}
