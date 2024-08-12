using Application.Interfaces.Repositories;
using Application.Sales.Commands.CreateSale;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Entities;
using Tests.Mock.Requests;

namespace Tests.Application.UnitTests.Sales.Commands;

public class CreateSaleTest
{
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly Mock<IProductRepository> _productRepository;
    private readonly Mock<ILogger<CreateSaleCommandHandler>> _logger;
    private readonly CreateSaleCommandHandler _createSaleCommandHandler;

    public CreateSaleTest()
    {
        _saleRepository = new Mock<ISaleRepository>();
        _productRepository = new Mock<IProductRepository>();
        _logger = new Mock<ILogger<CreateSaleCommandHandler>>();
        _createSaleCommandHandler = new(_saleRepository.Object, _productRepository.Object, _logger.Object);
    }

    [Fact]
    public async void ShouldCreateSale()
    {
        // arrange
        var command = new CreateSaleCommand { Sale = new SaleRequestMock().Default().Build() };

        var productNotInDb = new ProductMock().Default().Build()!;

        _productRepository.Setup(p =>
            p.GetByIdAsync(command.Sale!.Items![0].ProductId, It.IsAny<CancellationToken>())
        ).ReturnsAsync(productNotInDb);

        // act
        var result = await _createSaleCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        Assert.NotEqual(result, Guid.Empty);
    }

    [Fact]
    public async Task ShouldThrowException_WhenProductItemNotExistsInDataBase()
    {
        // arrange
        var command = new CreateSaleCommand { Sale = new SaleRequestMock().Default().Build() };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _createSaleCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ShouldThrowException_WhenProductHasNoStockQuantityEnough()
    {
        // arrange
        var command = new CreateSaleCommand { Sale = new SaleRequestMock().Default().WithItemQuantity(200).Build() };

        var productNotInDb = new ProductMock().Default().Build()!;

        _productRepository.Setup(p =>
            p.GetByIdAsync(command.Sale!.Items![0].ProductId, It.IsAny<CancellationToken>())
        ).ReturnsAsync(productNotInDb);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _createSaleCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }
}