using Application.Interfaces.Repositories;
using Application.Products.Maps;
using Application.Products.Queries.GetAllProducts;
using Domain.Entities;
using Domain.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Entities;

namespace Tests.Application.UnitTests.Products.Queries;

public class GetAllProductsTest
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ILogger<GetAllProductsQueryHandler>> _logger;
    private readonly GetAllProductsQueryHandler _getAllProductsCommandHandler;

    public GetAllProductsTest()
    {
        _productRepository = new Mock<IProductRepository>();
        _logger = new Mock<ILogger<GetAllProductsQueryHandler>>();
        _getAllProductsCommandHandler = new(_productRepository.Object, _logger.Object);
    }

    [Fact]
    public async void ShouldGetAllProducts()
    {
        // arrange
        var query = new GetAllProductsQuery { Filters = new() };

        var product = new ProductMock().Default().Build()!;

        List<Product> productsInDb = [product];

        _productRepository.Setup(p =>
            p.GetAllAsync(It.IsAny<ProductFilter>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(productsInDb);

        // act
        var result = await _getAllProductsCommandHandler.Handle(query, It.IsAny<CancellationToken>());

        var expectedProducts = productsInDb.Select(p => p.MapToProductDto()).ToList();

        // assert
        Assert.Equivalent(result, expectedProducts);
    }

    [Fact]
    public async Task ShouldThrowException_WhenMinPriceIsGreaterThanMaxPrice()
    {
        // arrange
        var query = new GetAllProductsQuery { Filters = new() { MinPrice = 10, MaxPrice = 1 } };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _getAllProductsCommandHandler.Handle(query, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ShouldThrowException_WhenMinStockQuantityIsGreaterThanMaxStockQuantity()
    {
        // arrange
        var query = new GetAllProductsQuery { Filters = new() { MinStockQuantity = 10, MaxStockQuantity = 1 } };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _getAllProductsCommandHandler.Handle(query, It.IsAny<CancellationToken>()));
    }
}