using System.Net;
using Application.Common.Extensions;
using Application.Sales.Commands.CancelSale;
using Application.Sales.Commands.CreateSale;
using Application.Sales.Dtos;
using Application.Sales.Queries.GetSaleShippingCost;
using Application.Sales.Queries.GetSalesHistory;
using Application.Sales.Requests;

namespace Web.Endpoints;

public static class Sales
{
    public static void MapSalesEndpoint(this WebApplication app)
    {
        app.MapGet("/sales-history", GetHistory)
           .Produces<List<SaleDto>>((int)HttpStatusCode.OK)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("GetSalesHistory");

        app.MapPost("/sales", Create)
           .Produces<Guid>((int)HttpStatusCode.OK)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("CreateSale");

        app.MapGet("/sales-shipping-cost", GetShippingCost)
           .Produces<double>((int)HttpStatusCode.OK)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("GetShippingCost");

        app.MapPost("/sales-cancel", Cancel)
           .Produces((int)HttpStatusCode.NoContent)
           .Produces((int)HttpStatusCode.BadRequest)
           .Produces((int)HttpStatusCode.InternalServerError)
           .WithName("CancelSale");
    }

    public static async Task<IResult> GetHistory(
        IMediator mediator,
        bool getItems,
        DateTime? minSaleDate,
        DateTime? maxSaleDate,
        string? status,
        decimal? minTotalAmount,
        decimal? maxTotalAmount,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return Results.Ok(await mediator.Send(
                new GetSalesHistoryQuery
                {
                    Filters = new()
                    {
                        GetItems = getItems,
                        MinSaleDate = minSaleDate,
                        MaxSaleDate = maxSaleDate,
                        Status = status,
                        MinTotalAmount = minTotalAmount,
                        MaxTotalAmount = maxTotalAmount
                    }
                }, cancellationToken
            ));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Create(
        IMediator mediator,
        SaleRequest request,
        ILogger<WebApplication> logger,
        CancellationToken cancellationToken
    )
    {
        if (!request.IsValid())
        {
            return LoggErrorsInternal(request, logger);
        }

        foreach (var item in request.Items!)
        {
            if (!item.IsValid())
            {
                return LoggErrorsInternal(request, logger);
            }
        }

        try
        {
            return Results.Ok(await mediator.Send(new CreateSaleCommand { Sale = request }, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            logger.LogError(ex.Message);
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> GetShippingCost(
        IMediator mediator,
        string zipCode,
        CancellationToken cancellationToken
    )
    {
        try
        {
            return Results.Ok(await mediator.Send(new GetSaleShippingCostQuery { ZipCode = zipCode }, cancellationToken));
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(ex.Message);
        }
    }

    public static async Task<IResult> Cancel(IMediator mediator, Guid id, CancellationToken cancellationToken)
    {
        await mediator.Send(new CancelSaleCommand { Id = id }, cancellationToken);

        return Results.NoContent();
    }

    private static IResult LoggErrorsInternal(SaleRequest request, ILogger<WebApplication> logger)
    {
        var errors = request.GetErrors();

        logger.LogError(errors);

        return Results.BadRequest(request.GetErrors()!);
    }
}