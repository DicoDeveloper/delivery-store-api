using Application.Interfaces.Repositories;
using Application.Products.Commands.CreateProduct;
using Microsoft.Extensions.Logging;

namespace Application.Products.Validators;

public static class CreateProductCommandValidator
{
    public static async Task ValidateAsync(
        this CreateProductCommand command,
        IProductRepository productRepository,
        ILogger<CreateProductCommandHandler> logger,
        CancellationToken cancellationToken = default
    )
    {
        if (await productRepository.IsThereProductWithNameAsync(command.Product.Name, cancellationToken: cancellationToken))
        {
            logger.LogError("There's already a product with the name {Name}", command.Product.Name);

            throw new ArgumentException($"Já existe um produto com o nome {command.Product.Name}.");
        }
    }
}