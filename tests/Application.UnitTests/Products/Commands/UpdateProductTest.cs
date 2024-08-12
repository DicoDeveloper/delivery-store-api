using Application.Interfaces.Repositories;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Maps;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Requests;

namespace Tests.Application.UnitTests.Products.Commands;

public class UpdateProductTest
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ILogger<UpdateProductCommandHandler>> _logger;
    private readonly UpdateProductCommandHandler _updateProductCommandHandler;

    public UpdateProductTest()
    {
        _productRepository = new Mock<IProductRepository>();
        _logger = new Mock<ILogger<UpdateProductCommandHandler>>();
        _updateProductCommandHandler = new(_productRepository.Object, _logger.Object);
    }

    [Fact]
    public async void ShouldUpdateProduct()
    {
        // arrange
        var command = new UpdateProductCommand { Product = new ProductRequestMock().Default().Build()! };

        var productInDb = command.Product.MapToProductEntity();

        _productRepository.Setup(p =>
            p.GetByIdAsync(command.Id, It.IsAny<CancellationToken>())
        ).ReturnsAsync(productInDb);

        // act
        var result = await _updateProductCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        Assert.Equal(result, Unit.Value);
    }

    [Fact]
    public async Task ShouldThrowException_WhenCommandIsInvalid()
    {
        // arrange
        var command = new UpdateProductCommand { Product = new ProductRequestMock().Default().Build()! };

        _productRepository.Setup(p =>
            p.IsThereProductWithNameAsync(command.Product.Name, null, It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _updateProductCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ShouldThrowException_WhenProductNotExistsInDataBase()
    {
        // arrange
        var command = new UpdateProductCommand { Product = new ProductRequestMock().Default().Build()! };

        // act & assert 
        await Assert.ThrowsAsync<ArgumentException>(() => _updateProductCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }
}