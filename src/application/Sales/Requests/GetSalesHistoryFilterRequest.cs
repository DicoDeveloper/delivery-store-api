namespace Application.Sales.Requests;

public class GetSalesHistoryFilterRequest
{
    public bool? GetItems { get; set; }
    public DateTime? MinSaleDate { get; set; }
    public DateTime? MaxSaleDate { get; set; }
    public string? Status { get; set; }
    public decimal? MinTotalAmount { get; set; }
    public decimal? MaxTotalAmount { get; set; }
}