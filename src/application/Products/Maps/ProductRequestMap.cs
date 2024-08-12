using Application.Products.Requests;
using Domain.Entities;
using Domain.Enums;
using Domain.Extensions;

namespace Application.Products.Maps;

public static class ProductRequestMap
{
    public static Product MapToProductEntity(this ProductRequest request)
        => new()
        {
            Name = request.Name,
            Price = request.Price,
            Category = request.Category!.StringToEnum<Category>(),
            Description = request.Description,
            StockQuantity = request.StockQuantity
        };
}