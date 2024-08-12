using Application.Common;

namespace Application.Products.Dtos;

public class ProductDto : BaseDto
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
    public int StockQuantity { get; set; }
}