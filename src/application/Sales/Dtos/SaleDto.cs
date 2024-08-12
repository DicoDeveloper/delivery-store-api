using Application.Common;

namespace Application.Sales.Dtos;

public class SaleDto : BaseDto
{
    public List<SaleItemDto> Items { get; set; } = [];
    public DateTime SaleDate { get; set; }
    public DateTime? CanceledDate { get; set; }
    public required string Status { get; set; }
    public decimal TotalAmount { get; set; }
}