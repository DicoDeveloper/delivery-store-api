using Application.Interfaces.Repositories;
using Application.Products.Commands.DeleteProduct;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application.UnitTests.Products.Commands;

public class DeleteProductTest
{
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ILogger<DeleteProductCommandHandler>> _logger;
    private readonly DeleteProductCommandHandler _deleteProductCommandHandler;

    public DeleteProductTest()
    {
        _productRepository = new Mock<IProductRepository>();
        _logger = new Mock<ILogger<DeleteProductCommandHandler>>();
        _deleteProductCommandHandler = new(_productRepository.Object, _logger.Object);
    }

    [Fact]
    public async void ShouldDeleteProduct()
    {
        // arrange
        var command = new DeleteProductCommand();

        _productRepository.Setup(p =>
           p.DeleteAsync(command.Id, It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        // act
        var result = await _deleteProductCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        Assert.True(result);
    }

    [Fact]
    public async Task ShouldNotDeleteProduct_WhenThereIsNoProductWithGivenId()
    {
        // arrange
        var command = new DeleteProductCommand();

        _productRepository.Setup(p =>
           p.DeleteAsync(command.Id, It.IsAny<CancellationToken>())
        ).ReturnsAsync(false);

        // act
        var result = await _deleteProductCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        Assert.False(result);
    }
}