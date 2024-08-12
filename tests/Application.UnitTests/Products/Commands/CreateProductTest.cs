using Application.Interfaces.Repositories;
using Application.Products.Commands.CreateProduct;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Requests;

namespace Tests.Application.UnitTests.Products.Commands;

public class CreateProductTest
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ILogger<CreateProductCommandHandler>> _logger;
    private readonly CreateProductCommandHandler _createProductCommandHandler;

    public CreateProductTest()
    {
        _productRepository = new Mock<IProductRepository>();
        _logger = new Mock<ILogger<CreateProductCommandHandler>>();
        _createProductCommandHandler = new(_productRepository.Object, _logger.Object);
    }

    [Fact]
    public async void ShouldCreateNewProduct()
    {
        // arrange
        var command = new CreateProductCommand { Product = new ProductRequestMock().Default().Build()! };

        // act
        var result = await _createProductCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        Assert.NotEqual(result, Guid.Empty);
    }

    [Fact]
    public async Task ShouldThrowException_WhenCommandIsInvalid()
    {
        // arrange
        var command = new CreateProductCommand { Product = new ProductRequestMock().Default().Build()! };

        _productRepository.Setup(p =>
            p.IsThereProductWithNameAsync(command.Product.Name, null, It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _createProductCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }
}