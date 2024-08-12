using Application.Common;

namespace Application.Sales.Dtos;

public class SaleItemDto : BaseDto
{
    public required string ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public required string Status { get; set; }
    public decimal TotalPrice { get; set; }
}