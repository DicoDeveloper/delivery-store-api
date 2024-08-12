using Application.Products.Queries.GetAllProducts;
using Microsoft.Extensions.Logging;

namespace Application.Products.Validators;

public static class GetAllProductsQueryValidator
{
    public static void Validate(this GetAllProductsQuery query, ILogger<GetAllProductsQueryHandler> logger)
    {
        if (query is not null && query.Filters is not null)
        {
            if (query.Filters.MinPrice > query.Filters.MaxPrice)
            {
                logger.LogError("MinPrice cannot be greater than the MaxPrice.");

                throw new ArgumentException("Valor mínimo não pode ser maior que o valor máximo.");
            }

            if (query.Filters.MinStockQuantity > query.Filters.MaxStockQuantity)
            {
                logger.LogError("MinStockQuantity cannot be greater than the MaxStockQuantity.");

                throw new ArgumentException("Quantidade mínima de estoque não pode ser maior que a quantidade máxima de estoque.");
            }
        }
    }
}