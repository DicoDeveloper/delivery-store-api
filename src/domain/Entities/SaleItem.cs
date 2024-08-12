using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid ProductId { get; set; }
    public Guid SaleId { get; set; }
    public Product? Product { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public SaleItemStatus Status { get; set; }
    public decimal TotalPrice => UnitPrice * Quantity;

    public void Cancel()
        => Status = SaleItemStatus.Canceled;
}