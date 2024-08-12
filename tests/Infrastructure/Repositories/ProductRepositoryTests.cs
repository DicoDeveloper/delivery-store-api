using Domain.Enums;
using Domain.Filters;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Tests.Mock.Entities;

namespace Tests.Infrastructure.Repositories;

public class ProductRepositoryTests : BaseRepositoryTests
{
    public ProductRepositoryTests() : base("ProductDatabase") { }

    [Fact]
    public async Task GetAllAsyncWithoutFilters_ShouldReturnProducts()
    {
        // arrange
        using var context = CreateContext();

        ClearDataBase(context);

        var product1 = new ProductMock().Default().Build()!;
        var product2 = new ProductMock().Default().Build()!;
        context.Products.Add(product1);
        context.Products.Add(product2);
        await context.SaveChangesAsync();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.GetAllAsync();

        // assert
        Assert.NotNull(result);
        Assert.True(result.Count == 2);
    }

    public static TheoryData<ProductFilter?> ProductFilters
       => new()
       {
            { new() { Name = "Teste 2" } },
            { new() { MinPrice = 110, MaxPrice = 112 } },
            { new() { Category = Category.Groceries } },
            { new() { Description = "Description Test 2" } },
            { new() { MinStockQuantity = 455, MaxStockQuantity = 457 } }
       };

    [Theory]
    [MemberData(nameof(ProductFilters))]
    public async Task GetAllAsyncWithFilters_ShouldReturnProduct(ProductFilter productFilter)
    {
        // arrange
        using var context = CreateContext();

        ClearDataBase(context);

        var product1 = new ProductMock().Default().Build()!;
        var product2 = new ProductMock().Default()
                                        .WithName("Teste 2")
                                        .WithSearchName("teste 2")
                                        .WithPrice(111)
                                        .WithCategory(Category.Groceries)
                                        .WithDescription("Description Test 2")
                                        .WithStockQuantity(456)
                                        .Build()!;
        context.Products.Add(product1);
        context.Products.Add(product2);
        await context.SaveChangesAsync();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.GetAllAsync(productFilter);

        // assert
        Assert.NotNull(result);
        Assert.True(result.Count == 1);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnProduct()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.GetByIdAsync(product.Id);

        // assert
        Assert.NotNull(result);
        Assert.Equivalent(product, result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenProductNotExists()
    {
        // arrange
        using var context = CreateContext();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // assert
        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_ShouldAddProduct()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;

        // act
        var repository = new ProductRepository(context);
        await repository.AddAsync(product);

        // assert
        var addedUser = await context.Products.FindAsync(product.Id);
        Assert.NotNull(addedUser);
        Assert.Equivalent(product, addedUser);
    }

    [Fact]
    public async Task AddAsync_ShouldProductUser_WhenProductExistsInDataBase()
    {
        // arrange
        using var context = CreateContext();

        var nameUpdated = "FirstNameUpdated2";
        var product = new ProductMock().Default().Build()!;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        product.Name = nameUpdated;

        // act
        var repository = new ProductRepository(context);
        await repository.AddAsync(product);

        // assert
        var updatedProduct = await context.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equivalent(product, updatedProduct);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateProduct()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        var nameUpdate = "TestProductUpdate";
        product.Name = nameUpdate;

        // act
        var repository = new ProductRepository(context);
        await repository.UpdateAsync(product);

        // assert
        var updatedProduct = await context.Products.FindAsync(product.Id);
        Assert.NotNull(updatedProduct);
        Assert.Equivalent(product, updatedProduct);
    }

    [Fact]
    public async Task UpdateAsync_ShouldAddProduct_WhenProductNotExistsInDataBase()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;

        // act
        var repository = new ProductRepository(context);
        await repository.UpdateAsync(product);

        // assert
        var addedProduct = await context.Products.FindAsync(product.Id);
        Assert.NotNull(addedProduct);
        Assert.Equal(product, addedProduct);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDeleteProduct()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // act
        var repository = new ProductRepository(context);
        await repository.DeleteAsync(product.Id);

        // assert
        var deletedProduct = await context.Products.FindAsync(product.Id);
        Assert.True(deletedProduct!.IsDeleted);
    }

    [Fact]
    public async Task DeleteAsync_ShouldDoNothing_WhenProductNotExistsInDataBase()
    {
        // arrange
        using var context = CreateContext();

        var id = Guid.NewGuid();

        // act
        var repository = new ProductRepository(context);
        await repository.DeleteAsync(id);

        // assert
        var deletedProduct = await context.Products.FindAsync(id);
        Assert.Null(deletedProduct);
    }

    [Fact]
    public async Task IsThereProductWithNameAsync_ShouldReturnFalse()
    {
        // arrange
        using var context = CreateContext();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.IsThereProductWithNameAsync("Name test");

        // assert
        Assert.False(result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("      ")]
    public async Task IsThereProductWithNameAsync_ShouldReturnFalse_WhenNameIsNullOrWhiteSpace(string invalidName)
    {
        // arrange
        using var context = CreateContext();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.IsThereProductWithNameAsync(invalidName);

        // assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsThereProductWithNameAsync_ShouldReturnTrue()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.IsThereProductWithNameAsync(product.Name);

        // assert
        Assert.True(result);
    }

    [Fact]
    public async Task IsThereProductWithNameAsync_ShouldReturnFalse_WithIdParameter()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.IsThereProductWithNameAsync("Name test", product.Id);
        
        // assert
        Assert.False(result);
    }

    [Fact]
    public async Task IsThereProductWithNameAsync_ShouldReturnTrue_WithIdParameter()
    {
        // arrange
        using var context = CreateContext();

        var product = new ProductMock().Default().Build()!;
        context.Products.Add(product);
        await context.SaveChangesAsync();

        // act
        var repository = new ProductRepository(context);
        var result = await repository.IsThereProductWithNameAsync(product.Name, Guid.NewGuid());

        // assert
        Assert.True(result);
    }

    private static void ClearDataBase(ApplicationDbContext context)
    {
        var allProducts = context.Products.ToList();
        context.Products.RemoveRange(allProducts);
        context.SaveChanges();
    }
}