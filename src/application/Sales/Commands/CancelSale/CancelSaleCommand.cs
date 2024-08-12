using Application.Common;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Commands.CancelSale;

public class CancelSaleCommand : BaseCommandQuery, IRequest<Unit>
{ }

public class CancelSaleCommandHandler : IRequestHandler<CancelSaleCommand, Unit>
{
    private readonly ISaleRepository _saleRepository;
    private readonly ILogger<CancelSaleCommandHandler> _logger;

    public CancelSaleCommandHandler(ISaleRepository saleRepository, ILogger<CancelSaleCommandHandler> logger)
    {
        _saleRepository = saleRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(CancelSaleCommand command, CancellationToken cancellationToken)
    {
        var sale = await _saleRepository.GetByIdAsync(command.Id, true, cancellationToken);

        if (sale is null)
        {
            _logger.LogError("Unable to find sale with Id {Id}", JsonConvert.SerializeObject(command.Id));

            throw new ArgumentException($"Não foi possível encontrar a venda com o Id: {command.Id}");
        }

        await _saleRepository.CancelSaleAsync(sale, cancellationToken);

        _logger.LogInformation("Sale canceled with Id {Id}", JsonConvert.SerializeObject(command.Id));

        return Unit.Value;
    }
}