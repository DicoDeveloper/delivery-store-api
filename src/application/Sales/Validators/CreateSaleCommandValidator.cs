using Application.Interfaces.Repositories;
using Application.Sales.Commands.CreateSale;
using Microsoft.Extensions.Logging;

namespace Application.Sales.Validators;

public static class CreateSaleCommandValidator
{
    public static async Task ValidateAsync(
        this CreateSaleCommand command,
        IProductRepository productRepository,
        ILogger<CreateSaleCommandHandler> logger,
        CancellationToken cancellationToken = default
    )
    {
        foreach (var item in command.Sale!.Items!)
        {
            var product = await productRepository.GetByIdAsync(item.ProductId, cancellationToken);

            if (product is null)
            {
                logger.LogError("There's no product with the ID {Name}", item.ProductId);

                throw new ArgumentException($"Não existe um produto com o Id: {item.ProductId}.");
            }

            if ((product.StockQuantity - item.Quantity) < 0)
            {
                logger.LogError("There's not enough stock for the product {Name}", product.Name);

                throw new ArgumentException($"Não existe estoque suficiente para o produto: {product.Name}.");
            }
        }
    }
}