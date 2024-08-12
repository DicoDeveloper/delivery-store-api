using Application.Interfaces.Repositories;
using Application.Common;
using Application.Products.Dtos;
using Application.Products.Maps;

namespace Application.Products.Queries.GetProductById;

public class GetProductByIdQuery : BaseCommandQuery, IRequest<ProductDto?>
{ }

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
        => _productRepository = productRepository;

    public async Task<ProductDto?> Handle(GetProductByIdQuery query, CancellationToken cancellationToken)
        => (await _productRepository.GetByIdAsync(query.Id, cancellationToken))?.MapToProductDto();
}