using AutoMapper;
using FluentAssertions;
using Moq;
using PriceTrackerCloud.Application.Interfaces;
using PriceTrackerCloud.Application.Interfaces.Repositories;
using PriceTrackerCloud.Application.Mappings;
using PriceTrackerCloud.Application.Queries.Products;
using PriceTrackerCloud.Domain.Entities;

namespace PriceTrackerCloud.Tests.Handlers;

public class GetProductsQueryHandlerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new();
    private readonly Mock<IProductRepository> _productRepoMock = new();
    private readonly IMapper _mapper;

    public GetProductsQueryHandlerTests()
    {
        _uowMock.Setup(u => u.Products).Returns(_productRepoMock.Object);
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>()).CreateMapper();
    }

    [Fact]
    public async Task Handle_WithProducts_ShouldReturnMappedDtos()
    {
        // Arrange
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Name = "PS5",    Category = "Videojuegos", Description = "Consola" },
            new() { Id = Guid.NewGuid(), Name = "iPhone", Category = "Smartphones", Description = "Móvil" }
        };
        _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(products);

        var handler = new GetProductsQueryHandler(_uowMock.Object, _mapper);

        // Act
        var result = (await handler.Handle(new GetProductsQuery(), CancellationToken.None)).ToList();

        // Assert
        result.Should().HaveCount(2);
        result[0].Name.Should().Be("PS5");
        result[1].Category.Should().Be("Smartphones");
    }

    [Fact]
    public async Task Handle_NoProducts_ShouldReturnEmptyList()
    {
        // Arrange
        _productRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync([]);

        var handler = new GetProductsQueryHandler(_uowMock.Object, _mapper);

        // Act
        var result = await handler.Handle(new GetProductsQuery(), CancellationToken.None);

        // Assert
        result.Should().BeEmpty();
    }
}
