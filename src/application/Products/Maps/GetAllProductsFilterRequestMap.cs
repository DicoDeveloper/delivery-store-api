using Application.Products.Requests;
using Domain.Enums;
using Domain.Extensions;
using Domain.Filters;

namespace Application.Products.Maps;

public static class GetAllProductsFilterRequestMap
{
    public static ProductFilter MapToProductFilter(this GetAllProductsFilterRequest request)
        => new()
        {
            Name = request.Name,
            MinPrice = request.MinPrice,
            MaxPrice = request.MaxPrice,
            Category = request.Category?.StringToEnum<Category>(),
            Description = request.Description,
            MinStockQuantity = request.MinStockQuantity,
            MaxStockQuantity = request.MaxStockQuantity
        };
}