using Domain.Enums;

namespace Domain.Filters;

public class SalesHistoryFilter
{
    public bool? GetItems { get; set; }
    public DateTime? MinSaleDate { get; set; }
    public DateTime? MaxSaleDate { get; set; }
    public SaleStatus? Status { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
}