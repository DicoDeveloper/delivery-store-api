using Application.Interfaces.Repositories;
using Application.Products.Maps;
using Application.Products.Queries.GetProductById;
using Moq;
using Tests.Mock.Entities;

namespace Tests.Application.UnitTests.Products.Queries;

public class GetProductByIdTest
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly GetProductByIdQueryHandler _getProductByIdCommandHandler;

    public GetProductByIdTest()
    {
        _productRepository = new Mock<IProductRepository>();
        _getProductByIdCommandHandler = new(_productRepository.Object);
    }

    [Fact]
    public async void ShouldGetProductById()
    {
        // arrange
        var query = new GetProductByIdQuery();

        var productInDb = new ProductMock().Default().Build()!;

        _productRepository.Setup(p =>
            p.GetByIdAsync(query.Id, It.IsAny<CancellationToken>())
        ).ReturnsAsync(productInDb);

        // act
        var result = await _getProductByIdCommandHandler.Handle(query, It.IsAny<CancellationToken>());

        var expectedProducts = productInDb.MapToProductDto();

        // assert
        Assert.Equivalent(result, expectedProducts);
    }

    [Fact]
    public async Task ShouldReturnNull_WhenProductNotExistsInDataBase()
    {
        // arrange
        var query = new GetProductByIdQuery();

        // act
        var result = await _getProductByIdCommandHandler.Handle(query, It.IsAny<CancellationToken>());

        // assert
        Assert.Null(result);
    }
}