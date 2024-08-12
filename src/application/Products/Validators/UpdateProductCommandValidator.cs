using Application.Interfaces.Repositories;
using Application.Products.Commands.UpdateProduct;
using Microsoft.Extensions.Logging;

namespace Application.Products.Validators;

public static class UpdateProductCommandValidator
{
    public static async Task ValidateAsync(
        this UpdateProductCommand command,
        IProductRepository productRepository,
        ILogger<UpdateProductCommandHandler> logger,
        CancellationToken cancellationToken = default
    )
    {
        if (await productRepository.IsThereProductWithNameAsync(command.Product.Name, command.Id, cancellationToken))
        {
            logger.LogError("There's already a product with the name {Name}", command.Product.Name);

            throw new ArgumentException($"Já existe um produto com o nome {command.Product.Name}.");
        }
    }
}