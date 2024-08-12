using Application.Sales.Requests;
using Domain.Enums;
using Domain.Extensions;
using Domain.Filters;

namespace Application.Sales.Maps;

public static class GetSalesHistoryFilterRequestMap
{
    public static SalesHistoryFilter MapToSalesHistoryFilter(this GetSalesHistoryFilterRequest request)
        => new()
        {
            GetItems = request.GetItems,
            MinSaleDate = request.MinSaleDate,
            MaxSaleDate = request.MaxSaleDate,
            Status = request.Status?.StringToEnum<SaleStatus>(),
            MinTotalAmount = request.MinTotalAmount,
            MaxTotalAmount = request.MaxTotalAmount
        };
}