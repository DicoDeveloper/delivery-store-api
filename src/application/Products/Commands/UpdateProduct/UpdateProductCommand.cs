using Application.Common;
using Application.Interfaces.Repositories;
using Application.Products.Extensions;
using Application.Products.Requests;
using Application.Products.Validators;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.UpdateProduct;

public class UpdateProductCommand : BaseCommandQuery, IRequest<Unit>
{
    public required ProductRequest Product { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Unit>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IProductRepository productRepository, ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Unit> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start UpdateProductCommand validation {Command}", JsonConvert.SerializeObject(command));

        await command.ValidateAsync(_productRepository, _logger, cancellationToken);

        var product = await _productRepository.GetByIdAsync(command.Id, cancellationToken);

        if (product is null)
        {
            _logger.LogError("There's no product with Id {Id}", command.Id);

            throw new ArgumentException($"Não existe um produto com o Id {command.Id}");
        }

        command.UpdateProductEntity(product);

        await _productRepository.UpdateAsync(product, cancellationToken);

        _logger.LogInformation("Product saved {Product}", JsonConvert.SerializeObject(product));

        return Unit.Value;
    }
}