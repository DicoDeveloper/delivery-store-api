using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class Product : BaseEntity
{
    public required string Name { get; set; }
    public string? SearchName { get; set; }
    public decimal Price { get; set; }
    public Category Category { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }

    public void SaleQuantity(int quantity)
        => StockQuantity -= quantity;
}