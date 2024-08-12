using Domain.Entities;
using Domain.Enums;

namespace Tests.Mock.Entities;

public class SaleMock
{
    private Sale? _sale;

    public SaleMock Default()
    {
        _sale = new()
        {
            SaleDate = DateTime.UtcNow,
            Status = SaleStatus.Created,
            Items = [
                new() {
                    ProductId = Guid.NewGuid(),
                    Quantity = 10,
                    UnitPrice = 50.25m,
                    Status = SaleItemStatus.Active
                }
            ]
        };

        _sale.SumTotalAmont();

        return this;
    }

    public SaleMock WithSaleDate(DateTime saleDate)
    {
        _sale!.SaleDate = saleDate;

        return this;
    }

    public SaleMock WithStatus(SaleStatus status)
    {
        _sale!.Status = status;

        return this;
    }

    public SaleMock WithItemQuantity(int quantity)
    {
        _sale!.Items[0].Quantity = quantity;

        _sale.SumTotalAmont();

        return this;
    }

    public SaleMock WithItemProduct(Product product)
    {
        _sale!.Items[0].Product = product;
        _sale!.Items[0].ProductId = product.Id;

        _sale.SumTotalAmont();

        return this;
    }

    public Sale? Build()
    {
        var sale = _sale;

        _sale = null;

        return sale;
    }
}