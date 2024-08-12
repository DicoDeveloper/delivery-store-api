using Application.Interfaces.Repositories;
using Application.Products.Maps;
using Application.Products.Requests;
using Application.Products.Validators;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<Guid>
{
    public required ProductRequest Product { get; set; }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Guid>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<CreateProductCommandHandler> _logger;

    public CreateProductCommandHandler(IProductRepository productRepository, ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<Guid> Handle(CreateProductCommand command, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Start CreateProductCommand validation {Command}", JsonConvert.SerializeObject(command));

        await command.ValidateAsync(_productRepository, _logger, cancellationToken);

        var newProduct = command.Product!.MapToProductEntity();

        await _productRepository.AddAsync(newProduct, cancellationToken);

        _logger.LogInformation("Product created: {Product}", JsonConvert.SerializeObject(newProduct));

        return newProduct.Id;
    }
}