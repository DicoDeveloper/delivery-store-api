using Application.Sales.Requests;
using Domain.Entities;

namespace Application.Sales.Maps;

public static class SaleItemRequestMap
{
    public static SaleItem MapToSaleEntity(this SaleItemRequest request)
        => new()
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity,
            UnitPrice = request.UnitPrice
        };
}