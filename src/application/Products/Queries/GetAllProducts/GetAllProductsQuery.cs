using Application.Interfaces.Repositories;
using Application.Products.Dtos;
using Application.Products.Maps;
using Application.Products.Requests;
using Application.Products.Validators;
using Microsoft.Extensions.Logging;

namespace Application.Products.Queries.GetAllProducts;

public class GetAllProductsQuery : IRequest<List<ProductDto>>
{
    public GetAllProductsFilterRequest? Filters { get; set; }
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, List<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<GetAllProductsQueryHandler> _logger;

    public GetAllProductsQueryHandler(IProductRepository productRepository, ILogger<GetAllProductsQueryHandler> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<List<ProductDto>> Handle(GetAllProductsQuery query, CancellationToken cancellationToken)
    {
        query.Validate(_logger);

        var result = await _productRepository.GetAllAsync(query.Filters?.MapToProductFilter(), cancellationToken);

        return result.Select(p => p.MapToProductDto()).ToList();
    }
}