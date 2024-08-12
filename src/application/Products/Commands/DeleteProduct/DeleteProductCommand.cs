using Application.Common;
using Application.Interfaces.Repositories;
using Microsoft.Extensions.Logging;

namespace Application.Products.Commands.DeleteProduct;

public class DeleteProductCommand : BaseCommandQuery, IRequest<bool>
{ }

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IProductRepository productRepository, ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand command, CancellationToken cancellationToken)
    {
        var isDeleted = await _productRepository.DeleteAsync(command.Id, cancellationToken);

        if (!isDeleted)
        {
            _logger.LogInformation("Delete is not possible, no product was found with the specified Id: {Id}", command.Id);
        }

        return isDeleted;
    }
}