using Application.Interfaces.Repositories;
using Application.Sales.Commands.CancelSale;
using Domain.Entities;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Application.UnitTests.Sales.Commands;

public class CancelSaleTest
{
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly Mock<ILogger<CancelSaleCommandHandler>> _logger;
    private readonly CancelSaleCommandHandler _cancelSaleCommandHandler;

    public CancelSaleTest()
    {
        _saleRepository = new Mock<ISaleRepository>();
        _logger = new Mock<ILogger<CancelSaleCommandHandler>>();
        _cancelSaleCommandHandler = new(_saleRepository.Object, _logger.Object);
    }

    [Fact]
    public async void ShouldCancelSale()
    {
        // arrange
        var command = new CancelSaleCommand();

        var saleInDb = new Sale();

        _saleRepository.Setup(p =>
            p.GetByIdAsync(command.Id, true, It.IsAny<CancellationToken>())
        ).ReturnsAsync(saleInDb);

        // act
        var result = await _cancelSaleCommandHandler.Handle(command, It.IsAny<CancellationToken>());

        // assert
        Assert.Equal(result, Unit.Value);
    }

    [Fact]
    public async Task ShouldThrowException_WhenSaleNotExistsInDataBase()
    {
        // arrange
        var command = new CancelSaleCommand();

        Sale? saleNotInDb = null;

        _saleRepository.Setup(p =>
            p.GetByIdAsync(command.Id, true, It.IsAny<CancellationToken>())
        ).ReturnsAsync(saleNotInDb);

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _cancelSaleCommandHandler.Handle(command, It.IsAny<CancellationToken>()));
    }
}