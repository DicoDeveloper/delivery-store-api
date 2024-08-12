using Application.Sales.Requests;
using Domain.Entities;

namespace Application.Sales.Maps;

public static class SaleRequestMap
{
    public static Sale MapToSaleEntity(this SaleRequest request)
        => new()
        {
            SaleDate = request.SaleDate!.Value,
            Items = request.Items?.Select(i => i.MapToSaleEntity()).ToList() ?? []
        };
}