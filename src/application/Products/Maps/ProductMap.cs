using Application.Products.Dtos;
using Domain.Entities;
using Domain.Extensions;

namespace Application.Products.Maps;

public static class ProductMap
{
    public static ProductDto MapToProductDto(this Product product)
        => new()
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category.GetDescription(),
            Description = product.Description,
            StockQuantity = product.StockQuantity
        };
}