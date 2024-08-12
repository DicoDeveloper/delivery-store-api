using Application.Sales.Dtos;
using Domain.Entities;
using Domain.Extensions;

namespace Application.Sales.Maps;

public static class SaleMap
{
    public static SaleDto MapToSaleDto(this Sale sale)
        => new()
        {
            Id = sale.Id,
            Status = sale.Status.GetDescription(),
            CanceledDate = sale.CanceledDate,
            SaleDate = sale.SaleDate,
            TotalAmount = sale.TotalAmount,
            Items = sale.Items?.Select(i => i.MapToSaleItemDto())?.ToList() ?? [],
        };
}