using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Sale : BaseEntity
{
    public List<SaleItem> Items { get; set; } = [];
    public DateTime SaleDate { get; set; }
    public DateTime? CanceledDate { get; set; }
    public SaleStatus Status { get; set; }
    public decimal TotalAmount { get; set; }

    public void Cancel()
    {
        Status = SaleStatus.Canceled;
        CanceledDate = DateTime.UtcNow;

        foreach (var item in Items)
        {
            item.Cancel();
        }
    }

    public void SumTotalAmont()
        => TotalAmount = Items.Sum(item => item.TotalPrice);
}