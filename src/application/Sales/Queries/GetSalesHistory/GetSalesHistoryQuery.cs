using Application.Interfaces.Repositories;
using Application.Sales.Dtos;
using Application.Sales.Maps;
using Application.Sales.Requests;
using Application.Sales.Validators;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Queries.GetSalesHistory;

public class GetSalesHistoryQuery : IRequest<List<SaleDto>>
{
    public GetSalesHistoryFilterRequest? Filters { get; set; }
}

public class GetSalesHistoryQueryHandler : IRequestHandler<GetSalesHistoryQuery, List<SaleDto>>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<GetSalesHistoryQueryHandler> _logger;

    public GetSalesHistoryQueryHandler(ISaleRepository saleRepository, ILogger<GetSalesHistoryQueryHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    public async Task<List<SaleDto>> Handle(GetSalesHistoryQuery query, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start GetSalesHistoryQuery validation {Query}", JsonConvert.SerializeObject(query));

        query.Validate(_logger);

        var result = await _saleRepository.GetAllAsync(query.Filters?.MapToSalesHistoryFilter(), cancellationToken);

        _logger.LogInformation("Number of sales found: {Count}", JsonConvert.SerializeObject(result.Count));

        return result.Select(p => p.MapToSaleDto()).ToList();
    }
}