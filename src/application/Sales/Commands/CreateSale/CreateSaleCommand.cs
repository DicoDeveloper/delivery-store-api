using Application.Interfaces.Repositories;
using Application.Sales.Maps;
using Application.Sales.Requests;
using Application.Sales.Validators;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Commands.CreateSale;

public class CreateSaleCommand : IRequest<Guid>
{
    public SaleRequest? Sale { get; set; }
}

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, Guid>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateSaleCommandHandler> _logger;

    public CreateSaleCommandHandler(
        ISaleRepository saleRepository,
        IProductRepository productRepository,
        ILogger<CreateSaleCommandHandler> logger
    )
    {
        _saleRepository = saleRepository;
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start CreateSaleCommand validation {Command}", JsonConvert.SerializeObject(command));

        await command.ValidateAsync(_productRepository, _logger, cancellationToken);

        var newSale = command.Sale!.MapToSaleEntity();

        await _saleRepository.ProcessSaleAsync(newSale, cancellationToken);

        _logger.LogInformation("Sale created {Sale}", JsonConvert.SerializeObject(newSale));

        return newSale.Id;
    }
}