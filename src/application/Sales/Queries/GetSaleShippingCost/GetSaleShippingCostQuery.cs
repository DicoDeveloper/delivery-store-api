using Application.Common.Extensions;
using Application.Interfaces.Repositories;
using Application.Sales.Validators;
using Application.ViaCep.Services;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Queries.GetSaleShippingCost;

public class GetSaleShippingCostQuery : IRequest<double>
{
    public required string ZipCode { get; set; }
}

public class GetSaleShippingCostQueryHandler : IRequestHandler<GetSaleShippingCostQuery, double>
{
    private readonly ICompanyRepository _companyRepository;
    private readonly IViaCepApiService _viaCepApiService;
    private readonly ILogger<GetSaleShippingCostQueryHandler> _logger;

    public GetSaleShippingCostQueryHandler(ICompanyRepository companyRepository, IViaCepApiService viaCepApiService, ILogger<GetSaleShippingCostQueryHandler> logger)
    {
        _companyRepository = companyRepository;
        _viaCepApiService = viaCepApiService;
        _logger = logger;
    }

    public async Task<double> Handle(GetSaleShippingCostQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start GetSaleShippingCostQuery validation {Query}", JsonConvert.SerializeObject(query));

        query.Validate(_logger);

        var mainCompany = await _companyRepository.GetMainCompanyAsync(cancellationToken);

        var viaCepInfo = await _viaCepApiService.GetInfoAsync(query.ZipCode.ClearToCompare(), cancellationToken);

        if (viaCepInfo?.Erro ?? false)
        {
            _logger.LogError("Invalid zip code");

            throw new ArgumentException("CEP inválido");
        }

        if (viaCepInfo is null || string.IsNullOrWhiteSpace(viaCepInfo.Localidade) || string.IsNullOrWhiteSpace(viaCepInfo.Uf))
        {
            _logger.LogError("Unable to find address data with zip code: {ShippingCost}", query.ZipCode);

            throw new ArgumentException($"Não foi possível encontrar os dados do endereço com o CEP: {query.ZipCode}");
        }

        var shippingCost = mainCompany?.ShippingCostOthersStates!.Value;

        if (viaCepInfo.Localidade!.ClearToCompare() == mainCompany?.Local.ClearToCompare())
        {
            shippingCost = mainCompany.ShippingCostSameLocal!.Value;
        }

        if (viaCepInfo.Uf!.ClearToCompare() == mainCompany?.State.ClearToCompare())
        {
            shippingCost = mainCompany.ShippingCostSameState!.Value;
        }

        _logger.LogInformation("Shipping cost finded {ShippingCost}", shippingCost);

        return shippingCost ?? 0;
    }
}