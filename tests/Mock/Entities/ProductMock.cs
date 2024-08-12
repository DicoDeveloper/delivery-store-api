using Domain.Entities;
using Domain.Enums;

namespace Tests.Mock.Entities;

public class ProductMock
{
    private Product? _product;

    public ProductMock Default()
    {
        _product = new()
        {
            Name = "Product test",
            Price = 10.5m,
            Category = Category.Clothing,
            Description = "Description test",
            StockQuantity = 100,
            SearchName = "product test",
        };

        return this;
    }

    public ProductMock WithName(string name)
    {
        _product!.Name = name;

        return this;
    }

    public ProductMock WithSearchName(string searchName)
    {
        _product!.SearchName = searchName;

        return this;
    }

    public ProductMock WithPrice(decimal price)
    {
        _product!.Price = price;

        return this;
    }

    public ProductMock WithCategory(Category category)
    {
        _product!.Category = category;

        return this;
    }

    public ProductMock WithDescription(string description)
    {
        _product!.Description = description;

        return this;
    }

    public ProductMock WithStockQuantity(int stockQuantity)
    {
        _product!.StockQuantity = stockQuantity;

        return this;
    }

    public Product? Build()
    {
        var product = _product;

        _product = null;

        return product;
    }
}