using Application.Common.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Domain.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Infrastructure.Repositories;

public class SaleRepository(IApplicationDbContext context, ILogger<SaleRepository> logger) : ISaleRepository
{
    private readonly IApplicationDbContext _context = context;
    private readonly ILogger<SaleRepository> _logger = logger;

    public async Task<List<Sale>> GetAllAsync(SalesHistoryFilter? filters = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.Where(p => !p.IsDeleted);

        if (filters is null)
        {
            return await query.OrderByDescending(q => q.UpdatedDate).ToListAsync(cancellationToken);
        }

        query = SetSaleHistoryFiltersToQuery(filters, query);

        return await query.OrderByDescending(q => q.UpdatedDate).ToListAsync(cancellationToken);
    }

    public async Task<Sale?> GetByIdAsync(Guid id, bool withItems = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsQueryable();

        if (withItems)
        {
            return await query.Include(q => q.Items).ThenInclude(i => i.Product).SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
        }

        return await query.AsNoTracking().SingleOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task AddAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await SaveInternal(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        var existingSale = await _context.Sales.SingleOrDefaultAsync(p => p.Id == sale.Id, cancellationToken);

        if (existingSale is null)
        {
            await AddAsync(sale, cancellationToken);

            return;
        }

        sale.UpdatedDate = DateTime.UtcNow;

        foreach (var item in sale.Items)
        {
            item.UpdatedDate = DateTime.UtcNow;
        }

        sale.SumTotalAmont();

        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Guid> ProcessSaleAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Updating sale status");

        UpdateStatuses(sale);

        _logger.LogInformation("Start save new sale {Sale}", JsonConvert.SerializeObject(sale));

        await SaveInternal(sale, cancellationToken);

        await AdjustStock(sale, isAdd: false, cancellationToken: cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return sale.Id;
    }

    public async Task CancelSaleAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        sale.Cancel();

        await AdjustStock(sale, isAdd: true, cancellationToken: cancellationToken);

        await UpdateAsync(sale, cancellationToken);
    }

    private static void UpdateStatuses(Sale newSale)
    {
        newSale.Status = SaleStatus.Created;

        foreach (var item in newSale.Items)
        {
            item.Status = SaleItemStatus.Active;
        }
    }

    private async Task AdjustStock(Sale sale, bool isAdd = false, CancellationToken cancellationToken = default)
    {
        foreach (var item in sale.Items)
        {
            var product = await _context.Products.SingleOrDefaultAsync(p => p.Id == item.ProductId, cancellationToken);
            product!.StockQuantity += isAdd ? item.Quantity : -item.Quantity;
            _context.Products.Update(product);

            _logger.LogInformation("Product stock adjusted, product: {ProductName}, stockQuantity: {StockQuantity}", product.Name, product.StockQuantity);
        }
    }

    private async Task SaveInternal(Sale sale, CancellationToken cancellationToken)
    {
        sale.CreatedDate = DateTime.UtcNow;

        foreach (var item in sale.Items)
        {
            item.CreatedDate = DateTime.UtcNow;
        }

        sale.SumTotalAmont();

        await _context.Sales.AddAsync(sale, cancellationToken);
    }

    private static IQueryable<Sale> SetSaleHistoryFiltersToQuery(SalesHistoryFilter filters, IQueryable<Sale> query)
    {
        if (filters.GetItems ?? false)
        {
            query = query.Include(s => s.Items).ThenInclude(i => i.Product);
        }

        if (filters.MinSaleDate is not null && filters.MinSaleDate.Value > DateTime.MinValue)
        {
            query = query.Where(s => s.SaleDate >= filters.MinSaleDate.Value);
        }

        if (filters.MaxSaleDate is not null && filters.MaxSaleDate.Value > DateTime.MinValue)
        {
            query = query.Where(s => s.SaleDate <= filters.MaxSaleDate.Value);
        }

        if (filters.Status is not null && filters.Status != SaleStatus.Undefined)
        {
            query = query.Where(s => s.Status == filters.Status);
        }

        if (filters.MinTotalAmount is not null && filters.MinTotalAmount.Value > 0)
        {
            query = query.Where(s => s.TotalAmount >= filters.MinTotalAmount.Value);
        }

        if (filters.MaxTotalAmount is not null && filters.MaxTotalAmount.Value > 0)
        {
            query = query.Where(s => s.TotalAmount <= filters.MaxTotalAmount.Value);
        }

        return query;
    }
}