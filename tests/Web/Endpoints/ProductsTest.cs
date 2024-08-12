using System.Net;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Queries.GetAllProducts;
using Application.Products.Requests;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Requests;
using Web.Endpoints;

namespace Tests.Web.Endpoints;

public class ProductsTest
{
    private readonly Mock<IMediator> _mediator;
    private readonly Mock<ILogger<WebApplication>> _logger;

    public ProductsTest()
    {
        _mediator = new Mock<IMediator>();
        _logger = new Mock<ILogger<WebApplication>>();
    }

    public static TheoryData<GetAllProductsFilterRequest> ProductFilters
       => new()
       {
            { new() { Name = "Teste 2" } },
            { new() { MinPrice = 110, MaxPrice = 112 } },
            { new() { Category = "Livros" } },
            { new() { Description = "Description Test 2" } },
            { new() { MinStockQuantity = 455, MaxStockQuantity = 457 } }
       };

    [Theory]
    [MemberData(nameof(ProductFilters))]
    public async Task GetAll_ShouldReturnOk(GetAllProductsFilterRequest filters)
    {
        // arrange & act
        var response = await Products.GetAll(_mediator.Object, filters.Name, filters.MinPrice, filters.MaxPrice, filters.Category, filters.Description, filters.MinStockQuantity, filters.MaxStockQuantity, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.OK, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task GetAll_ShouldReturnBadRequest()
    {
        // arrange
        _mediator.Setup(p =>
            p.Send(It.IsAny<GetAllProductsQuery>(), It.IsAny<CancellationToken>())
        ).Throws(new ArgumentException());

        // act
        var response = await Products.GetAll(_mediator.Object, null, null, null, null, null, null, null, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk()
    {
        // arrange
        var id = Guid.NewGuid();

        // act
        var response = await Products.GetById(_mediator.Object, id, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.OK, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnOk()
    {
        // arrange
        var productRequest = new ProductRequestMock().Default().Build()!;

        // act
        var response = await Products.Create(_mediator.Object, productRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.OK, ((IStatusCodeHttpResult)response).StatusCode);
    }

    public static TheoryData<ProductRequest> InvalidProductRequest
       => new()
       {
            { new() { Name = "", Category = "" } },
            { new() { Name = "Test", Category = "" } },
            { new() { Name = "Test", Category = "Test", Price = -10 } },
            { new() { Name = "Test", Category = "Test", Price = 10, StockQuantity = -10 } }
       };

    [Theory]
    [MemberData(nameof(InvalidProductRequest))]
    public async Task Create_ShouldReturnBadRequest(ProductRequest invalidProductRequest)
    {
        // arrange & act
        var response = await Products.Create(_mediator.Object, invalidProductRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnBadRequest_WhenMediatorThrowsException()
    {
        // arrange
        var productRequest = new ProductRequestMock().Default().Build()!;

        _mediator.Setup(p =>
            p.Send(It.IsAny<CreateProductCommand>(), It.IsAny<CancellationToken>())
        ).Throws(new ArgumentException());

        // act
        var response = await Products.Create(_mediator.Object, productRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnOk()
    {
        // arrange
        var productRequest = new ProductRequestMock().Default().Build()!;

        // act
        var response = await Products.Update(_mediator.Object, Guid.NewGuid(), productRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.NoContent, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Theory]
    [MemberData(nameof(InvalidProductRequest))]
    public async Task Update_ShouldReturnBadRequest(ProductRequest invalidProductRequest)
    {
        // arrange & act
        var response = await Products.Update(_mediator.Object, Guid.NewGuid(), invalidProductRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenMediatorThrowsException()
    {
        // arrange
        var productRequest = new ProductRequestMock().Default().Build()!;

        _mediator.Setup(p =>
            p.Send(It.IsAny<UpdateProductCommand>(), It.IsAny<CancellationToken>())
        ).Throws(new ArgumentException());

        // act
        var response = await Products.Update(_mediator.Object, Guid.NewGuid(), productRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturnBadRequest_WhenIdIsEmptyGuid()
    {
        // arrange
        var productRequest = new ProductRequestMock().Default().Build()!;

        // act
        var response = await Products.Update(_mediator.Object, Guid.Empty, productRequest, _logger.Object, CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnOk()
    {
        // arrange
        _mediator.Setup(p =>
            p.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(true);

        // act
        var response = await Products.Delete(_mediator.Object, Guid.NewGuid(), CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.NoContent, ((IStatusCodeHttpResult)response).StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturnBadRequest()
    {
        // arrange
        var productRequest = new ProductRequestMock().Default().Build()!;

        _mediator.Setup(p =>
            p.Send(It.IsAny<DeleteProductCommand>(), It.IsAny<CancellationToken>())
        ).ReturnsAsync(false);

        // act
        var response = await Products.Delete(_mediator.Object, Guid.NewGuid(), CancellationToken.None);

        // assert
        Assert.Equal((int)HttpStatusCode.BadRequest, ((IStatusCodeHttpResult)response).StatusCode);
    }
}