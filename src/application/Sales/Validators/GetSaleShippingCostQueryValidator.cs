using System.Text.RegularExpressions;
using Application.Sales.Queries.GetSaleShippingCost;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Validators;

public static partial class GetSaleShippingCostQueryValidator
{
    private const string CepPattern = @"^\d{5}-?\d{3}$";

    public static void Validate(this GetSaleShippingCostQuery query, ILogger<GetSaleShippingCostQueryHandler> logger)
    {
        if (string.IsNullOrWhiteSpace(query.ZipCode))
        {
            logger.LogError("Zip code is required.");

            throw new ArgumentException("Informe o CEP.");
        }

        if (!CepRegex().IsMatch(query.ZipCode))
        {
            logger.LogError("Invalid zip code format: {ZipCode}", query.ZipCode);

            throw new ArgumentException("Formato inválido de CEP. Use XXXXX-XXX ou XXXXXXXX.");
        }
    }

    [GeneratedRegex(CepPattern)]
    private static partial Regex CepRegex();
}