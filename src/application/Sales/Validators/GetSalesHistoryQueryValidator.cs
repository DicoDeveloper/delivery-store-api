using Application.Sales.Queries.GetSalesHistory;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Validators;

public static class GetSalesHistoryQueryValidator
{
    public static void Validate(this GetSalesHistoryQuery query, ILogger<GetSalesHistoryQueryHandler> logger)
    {
        if (query is not null && query.Filters is not null)
        {
            if (query.Filters.MinSaleDate > query.Filters.MaxSaleDate)
            {
                logger.LogInformation("Minimum sale date cannot be greater than the maximum sale date.");

                throw new ArgumentException($"Data mínima da venda não pode ser maior que a data máxima da venda.");
            }

            if (query.Filters.MinTotalAmount > query.Filters.MaxTotalAmount)
            {
                logger.LogInformation("Minimum value of the total sale cannot be greater than the maximum value of the total sale.");

                throw new ArgumentException($"Valor mínima do total da venda não pode ser maior que o valor máxima do total da venda.");
            }
        }
    }
}