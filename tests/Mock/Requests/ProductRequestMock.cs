using Application.Products.Requests;

namespace Tests.Mock.Requests;

public class ProductRequestMock
{
    private ProductRequest? _productRequest;

    public ProductRequestMock Default()
    {
        _productRequest = new()
        {
            Name = "Product test",
            Price = 10.5m,
            Category = "Livros",
            Description = "Description test",
            StockQuantity = 100
        };

        return this;
    }

    public ProductRequest? Build()
    {
        var productRequest = _productRequest;

        _productRequest = null;

        return productRequest;
    }
}