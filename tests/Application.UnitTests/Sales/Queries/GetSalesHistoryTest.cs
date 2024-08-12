using Application.Interfaces.Repositories;
using Application.Sales.Maps;
using Application.Sales.Queries.GetSalesHistory;
using Domain.Entities;
using Domain.Filters;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Entities;
using Tests.Mock.Requests;

namespace Tests.Application.UnitTests.Sales.Queries;

public class GetSalesHistoryTest
{
    private readonly Mock<ISaleRepository> _saleRepository;
    private readonly Mock<ILogger<GetSalesHistoryQueryHandler>> _logger;
    private readonly GetSalesHistoryQueryHandler _getSalesHistoryCommandHandler;

    public GetSalesHistoryTest()
    {
        _saleRepository = new Mock<ISaleRepository>();
        _logger = new Mock<ILogger<GetSalesHistoryQueryHandler>>();
        _getSalesHistoryCommandHandler = new(_saleRepository.Object, _logger.Object);
    }

    [Fact]
    public async void ShouldGetSalesHistory()
    {
        // arrange
        var query = new GetSalesHistoryQuery { Filters = new() };

        List<Sale> salesInDb = [new SaleRequestMock().Default().Build()!.MapToSaleEntity()];

        salesInDb[0].Items[0].Product = new ProductMock().Default().Build()!;

        _saleRepository.Setup(p =>
            p.GetAllAsync(It.IsAny<SalesHistoryFilter>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(salesInDb);

        // act
        var result = await _getSalesHistoryCommandHandler.Handle(query, It.IsAny<CancellationToken>());

        var expectedSalesHistory = salesInDb.Select(p => p.MapToSaleDto()).ToList();

        // assert
        Assert.Equivalent(result, expectedSalesHistory);
    }

    [Fact]
    public async Task ShouldThrowException_WhenMinPriceIsGreaterThanMaxPrice()
    {
        // arrange
        var query = new GetSalesHistoryQuery { Filters = new() { MinSaleDate = DateTime.UtcNow, MaxSaleDate = DateTime.UtcNow.AddDays(-1) } };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _getSalesHistoryCommandHandler.Handle(query, It.IsAny<CancellationToken>()));
    }

    [Fact]
    public async Task ShouldThrowException_WhenMinStockQuantityIsGreaterThanMaxStockQuantity()
    {
        // arrange
        var query = new GetSalesHistoryQuery { Filters = new() { MinTotalAmount = 10, MaxTotalAmount = 1 } };

        // act & assert
        await Assert.ThrowsAsync<ArgumentException>(() => _getSalesHistoryCommandHandler.Handle(query, It.IsAny<CancellationToken>()));
    }
}