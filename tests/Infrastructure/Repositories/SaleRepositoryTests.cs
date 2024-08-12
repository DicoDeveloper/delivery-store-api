using Domain.Enums;
using Domain.Filters;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Microsoft.Extensions.Logging;
using Moq;
using Tests.Mock.Entities;

namespace Tests.Infrastructure.Repositories;

public class SaleRepositoryTests : BaseRepositoryTests
{
    private readonly Mock<ILogger<SaleRepository>> _logger;

    public SaleRepositoryTests() : base("SaleDatabase")
        => _logger = new Mock<ILogger<SaleRepository>>();

    [Fact]
    public async Task GetAllAsyncWithoutFilters_ShouldReturnSales()
    {
        // arrange
        using var context = CreateContext();

        ClearDataBase(context);

        var sale1 = new SaleMock().Default().Build()!;
        var sale2 = new SaleMock().Default().Build()!;
        context.Sales.Add(sale1);
        context.Sales.Add(sale2);
        await context.SaveChangesAsync();

        // act
        var repository = new SaleRepository(context, _logger.Object);
        var result = await repository.GetAllAsync();

        // assert
        Assert.NotNull(result);
        Assert.True(result.Count == 2);
    }

    public static TheoryData<SalesHistoryFilter?> SaleFilters
       => new()
       {
            { new() { MinSaleDate = DateTime.UtcNow.AddDays(-11), MaxSaleDate = DateTime.UtcNow.AddDays(-9) } },
            { new() { MinTotalAmount = 5000, MaxTotalAmount = 5026 } },
            { new() { Status = SaleStatus.Paid } }
       };

    [Theory]
    [MemberData(nameof(SaleFilters))]
    public async Task GetAllAsyncWithFilters_ShouldReturnSale(SalesHistoryFilter? saleFilter)
    {
        // arrange
        using var context = CreateContext();

        ClearDataBase(context);

        var sale1 = new SaleMock().Default().Build()!;
        var sale2 = new SaleMock().Default()
                                  .WithSaleDate(DateTime.UtcNow.AddDays(-10))
                                  .WithItemQuantity(100)
                                  .WithStatus(SaleStatus.Paid)
                                  .Build()!;
        context.Sales.Add(sale1);
        context.Sales.Add(sale2);
        await context.SaveChangesAsync();

        // act
        var repository = new SaleRepository(context, _logger.Object);
        var result = await repository.GetAllAsync(saleFilter);

        // assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSale()
    {
        // arrange
        using var context = CreateContext();

        var sale = new SaleMock().Default().Build()!;
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        // act
        var repository = new SaleRepository(context, _logger.Object);
        var result = await repository.GetByIdAsync(sale.Id, false);

        // assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnSale_WithItems()
    {
        // arrange
        using var context = CreateContext();

        var sale = new SaleMock().Default().Build()!;
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        // act
        var repository = new SaleRepository(context, _logger.Object);
        var result = await repository.GetByIdAsync(sale.Id, true);

        // assert
        Assert.NotNull(result);
        Assert.Equivalent(sale, result);
        Assert.NotEmpty(result.Items);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenSaleNotExists()
    {
        // arrange
        using var context = CreateContext();

        // act
        var repository = new SaleRepository(context, _logger.Object);
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddSale()
    {
        // arrange
        using var context = CreateContext();

        var sale = new SaleMock().Default().Build()!;

        // act
        var repository = new SaleRepository(context, _logger.Object);
        await repository.AddAsync(sale);

        // assert
        var addedSale = await context.Sales.FindAsync(sale.Id);
        Assert.NotNull(addedSale);
        Assert.Equivalent(sale, addedSale);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateSale()
    {
        // arrange
        using var context = CreateContext();

        var sale = new SaleMock().Default().Build()!;
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        var statusUpdated = SaleStatus.Delivered;
        sale.Status = statusUpdated;

        // act
        var repository = new SaleRepository(context, _logger.Object);
        await repository.UpdateAsync(sale);

        // assert
        var updatedSale = await context.Sales.FindAsync(sale.Id);
        Assert.NotNull(updatedSale);
        Assert.Equivalent(sale, updatedSale);
    }

    [Fact]
    public async Task UpdateAsync_ShouldAddSale_WhenSaleNotExistsInDataBase()
    {
        // arrange
        using var context = CreateContext();

        var sale = new SaleMock().Default().Build()!;

        // act
        var repository = new SaleRepository(context, _logger.Object);
        await repository.UpdateAsync(sale);

        // assert
        var addedSale = await context.Sales.FindAsync(sale.Id);
        Assert.NotNull(addedSale);
        Assert.Equal(sale, addedSale);
    }

    [Fact]
    public async Task CancelSaleAsync_ShouldCancelSale()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().WithStockQuantity(5).Build()!;

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var sale = new SaleMock().Default()
                                 .WithItemProduct(product)
                                 .WithItemQuantity(5)
                                 .Build()!;
        context.Sales.Add(sale);
        await context.SaveChangesAsync();

        // act
        var repository = new SaleRepository(context, _logger.Object);
        await repository.CancelSaleAsync(sale);

        // assert
        var canceledSale = await context.Sales.FindAsync(sale.Id);
        var productUpdated = await context.Products.FindAsync(sale.Items[0].ProductId);
        Assert.Equal(SaleStatus.Canceled, canceledSale!.Status);
         Assert.Equal(10, productUpdated!.StockQuantity);
    }

    [Fact]
    public async Task ProcessSale_ShouldCreateSaleAndUpdateProductStockQuantity()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().WithStockQuantity(10).Build()!;

        context.Products.Add(product);
        await context.SaveChangesAsync();

        var sale = new SaleMock().Default()
                                 .WithItemProduct(product)
                                 .WithItemQuantity(5)
                                 .Build()!;

        // act
        var repository = new SaleRepository(context, _logger.Object);
        var result = await repository.ProcessSaleAsync(sale);

        // assert
        var addedSale = await context.Sales.FindAsync(sale.Id);
        var productUpdated = await context.Products.FindAsync(sale.Items[0].ProductId);
        Assert.NotNull(addedSale);
        Assert.Equivalent(sale, addedSale);
        Assert.Equal(5, productUpdated!.StockQuantity);
    }

    private static void ClearDataBase(ApplicationDbContext context)
    {
        var allSales = context.Sales.ToList();
        context.Sales.RemoveRange(allSales);
        context.SaveChanges();
    }
}