using Application.Sales.Requests;

namespace Tests.Mock.Requests;

public class SaleRequestMock
{
    private SaleRequest? _saleRequest;

    public SaleRequestMock Default()
    {
        _saleRequest = new()
        {
            SaleDate = DateTime.UtcNow,
            Items = [
                new() {
                    ProductId = Guid.NewGuid(),
                    Quantity = 10,
                    UnitPrice = 10.50m,
                }
             ]
        };

        return this;
    }

    public SaleRequestMock WithItemQuantity(int quantity)
    {
        _saleRequest!.Items![0].Quantity = quantity;

        return this;
    }

    public SaleRequest? Build()
    {
        var saleRequest = _saleRequest;

        _saleRequest = null;

        return saleRequest;
    }
}