using System.Net;
using Application.Sales.Commands.CreateSale;
using Application.Sales.Queries.GetSalesHistory;
using Application.Sales.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Requests;
using Web.Endpoints;

namespace Tests.Web.Endpoints;

public class SalesTest
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<WebApplication>> _logger;

    public SalesTest()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<WebApplication>>();
    }

    public static TheoryData<GetSalesHistoryFilterRequest> SaleFilters
       => new()
       {
            { new() { MinSaleDate = DateTime.UtcNow.AddDays(-11), MaxSaleDate = DateTime.UtcNow.AddDays(-9) } },
            { new() { MinTotalAmount = 5000, MaxTotalAmount = 5026 } },
            { new() { Status = "Paga" } }
       };

    [Theory]
    [MemberData(nameof(SaleFilters))]
    public async Task GetHistory_ShouldReturnOk(GetSalesHistoryFilterRequest filters)
    {
        // arrange & act
        var response = await Sales.GetHistory(_mediator.Object, true, filters.MinSaleDate, filters.MaxSaleDate, filters.Status, filters.MinTotalAmount, filters.MaxTotalAmount, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.OK, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task GetHistory_ShouldReturnBadRequest()
    {
        // arrange
        _mediator.Setup(p =>
            p.Send(It.IsAny<GetSalesHistoryQuery>(), It.IsAny<CancellationToken>())
        ).Throws(new ArgumentException());

        // act
        var response = await Sales.GetHistory(_mediator.Object, true, null, null, null, null, null, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnOk()
    {
        // arrange
        var SaleRequest = new SaleRequestMock().Default().Build()!;

        // act
        var response = await Sales.Create(_mediator.Object, SaleRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.OK, ((IStatusCodeHttpResult)response).StatusCode);
    }

    public static TheoryData<SaleRequest> InvalidSaleRequest
       => new()
       {
            { new() { SaleDate = null } },
            { new() { SaleDate = DateTime.UtcNow, Items = null } },
            { new() { SaleDate = DateTime.UtcNow, Items = [ new() { ProductId = Guid.Empty } ] } },
            { new() { SaleDate = DateTime.UtcNow, Items = [ new() { ProductId = Guid.NewGuid(), Quantity = -10 } ] } },
            { new() { SaleDate = DateTime.UtcNow, Items = [ new() { ProductId = Guid.NewGuid(), Quantity = 10, UnitPrice = -100 } ] } },
       };

    [Theory]
    [MemberData(nameof(InvalidSaleRequest))]
    public async Task Create_ShouldReturnBadRequest(SaleRequest invalidSaleRequest)
    {
        // arrange & act
        var response = await Sales.Create(_mediator.Object, invalidSaleRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenMediatorThrowsException()
    {
        // arrange
        var SaleRequest = new SaleRequestMock().Default().Build()!;

        _mediator.Setup(p =>
            p.Send(It.IsAny<CreateSaleCommand>(), It.IsAny<CancellationToken>())
        ).Throws(new ArgumentException());

        // act
        var response = await Sales.Create(_mediator.Object, SaleRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task GetShippingCost_ShouldReturnOk()
    {
        // arrange & act
        var response = await Sales.GetShippingCost(_mediator.Object, "", CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.OK, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Cancel_ShouldReturnOk()
    {
        // arrange & act
        var response = await Sales.Cancel(_mediator.Object, Guid.NewGuid(), CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.NoContent, ((IStatusCodeHttpResult)response).StatusCode);
    }
}