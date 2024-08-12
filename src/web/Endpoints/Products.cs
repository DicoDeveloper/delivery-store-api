using System.Net;
using Application.Common.Extensions;
using Application.Products.Commands.CreateProduct;
using Application.Products.Commands.DeleteProduct;
using Application.Products.Commands.UpdateProduct;
using Application.Products.Dtos;
using Application.Products.Queries.GetAllProducts;
using Application.Products.Queries.GetProductById;
using Application.Products.Requests;

namespace Web.Endpoints;

public static class Products
{
    public static void MapProductsEndpoint(this WebApplication app)
    {
        app.MapGet("/products", GetAll)
           .Produces<List<ProductDto>>((int)HttpStatusCode.OK)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("GetAllProducts");

        app.MapGet("/products/{id:guid}", GetById)
           .Produces<ProductDto?>((int)HttpStatusCode.OK)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("GetProductById");

        app.MapPost("/products", Create)
           .Produces<Guid>((int)HttpStatusCode.OK)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("CreateProduct");

        app.MapPut("/products", Update)
           .Produces((int)HttpStatusCode.NoContent)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("UpdateProduct");

        app.MapDelete("/products", Delete)
           .Produces((int)HttpStatusCode.NoContent)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("DeleteProduct");
    }

    public static async Task<IResult> GetAll(
        IMediator mediator,
        string? name,
        decimal? minPrice,
        decimal? maxPrice,
        string? category,
        string? description,
        int? minStockQuantity,
        int? maxStockQuantity,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return Results.Ok(await mediator.Send(
                new GetAllProductsQuery
                {
                    Filters = new()
                    {
                        Name = name,
                        MinPrice = minPrice,
                        MaxPrice = maxPrice,
                        Category = category,
                        Description = description,
                        MinStockQuantity = minStockQuantity,
                        MaxStockQuantity = maxStockQuantity
                    }
                }, cancellationToken
            ));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetById(IMediator mediator, Guid id, CancellationToken cancellationToken)
        => Results.Ok(await mediator.Send(new GetProductByIdQuery { Id = id }, cancellationToken));

    public static async Task<IResult> Create(
        IMediator mediator,
        ProductRequest request,
        ILogger<WebApplication> logger,
        CancellationToken cancellationToken
    )
    {
        if (!request.IsValid())
        {
            var errors = request.GetErrors();

            logger.LogError(errors);

            return Results.BadRequest(errors);
        }

        try
        {
            return Results.Ok(await mediator.Send(new CreateProductCommand { Product = request }, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Update(
        IMediator mediator,
        Guid id,
        ProductRequest request,
        ILogger<WebApplication> logger,
        CancellationToken cancellationToken
    )
    {
        if (!request.IsValid())
        {
            var errors = request.GetErrors();

            logger.LogError(errors);

            return Results.BadRequest(request.GetErrors()!);
        }

        try
        {
            if (id == Guid.Empty)
            {
                return Results.BadRequest();
            }

            await mediator.Send(new UpdateProductCommand { Id = id, Product = request }, cancellationToken);

            return Results.NoContent();
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex.Message);
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Delete(IMediator mediator, Guid id, CancellationToken cancellationToken)
    {
        var isDeleted = await mediator.Send(new DeleteProductCommand { Id = id }, cancellationToken);

        if (!isDeleted)
        {
            return Results.BadRequest("Não foi encontrado nenhum produto com o id informado.");
        }

        return Results.NoContent();
    }
}