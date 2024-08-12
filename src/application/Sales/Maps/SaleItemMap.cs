using Application.Sales.Dtos;
using Domain.Entities;
using Domain.Extensions;

namespace Application.Sales.Maps;

public static class SaleItemMap
{
    public static SaleItemDto MapToSaleItemDto(this SaleItem saleItem)
        => new()
        {
            Id = saleItem.Id,
            Quantity = saleItem.Quantity,
            TotalPrice = saleItem.TotalPrice,
            UnitPrice = saleItem.UnitPrice,
            Status = saleItem.Status.GetDescription(),
            ProductName = saleItem.Product!.Name,
        };
}