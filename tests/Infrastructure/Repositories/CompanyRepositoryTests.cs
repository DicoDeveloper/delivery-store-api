using Infrastructure.Data;
using Infrastructure.Repositories;
using Tests.Mock.Entities;

namespace Tests.Infrastructure.Repositories;

public class CompanyRepositoryTests : BaseRepositoryTests
{
    public CompanyRepositoryTests() : base("CompanyDatabase") { }

    [Fact]
    public async Task GetMainCompanyAsync_ShouldReturnCompany()
    {
        // arrange
        using var context = CreateContext();

        var company = new CompanyMock().Default().Build()!;
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // act
        var repository = new CompanyRepository(context);
        var result = await repository.GetMainCompanyAsync();

        // assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenThereIsNoMainCompanyInDataBase()
    {
        // arrange
        using var context = CreateContext();

        ClearDataBase(context);

        var company = new CompanyMock().Default().WithIsMainHeadquarter(false).Build()!;
        context.Companies.Add(company);
        await context.SaveChangesAsync();

        // act
        var repository = new CompanyRepository(context);
        var result = await repository.GetMainCompanyAsync();

        // assert
        Assert.Null(result);
    }

    private static void ClearDataBase(ApplicationDbContext context)
    {
        var allProducts = context.Products.ToList();
        context.Products.RemoveRange(allProducts);
        context.SaveChanges();
    }
}