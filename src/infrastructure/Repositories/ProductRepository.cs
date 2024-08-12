using Application.Common.Extensions;
using Application.Common.Interfaces;
using Application.Interfaces.Repositories;
using Domain.Entities;
using Domain.Enums;
using Domain.Filters;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository(IApplicationDbContext context) : IProductRepository
{
    private readonly IApplicationDbContext _context = context;

    public async Task<List<Product>> GetAllAsync(ProductFilter? filters = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Products.Where(p => !p.IsDeleted);

        if (filters is null)
        {
            return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
        }

        query = SetProductFiltersToQuery(filters, query);

        return await query.OrderBy(p => p.Name).ToListAsync(cancellationToken);
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => await _context.Products.SingleOrDefaultAsync(p => !p.IsDeleted && p.Id == id, cancellationToken);

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existingProduct = await _context.Products.SingleOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        if (existingProduct is not null)
        {
            await UpdateAsync(product, cancellationToken);

            return;
        }

        product.SearchName = product.Name.ClearToCompare();
        product.CreatedDate = DateTime.UtcNow;

        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        var existingProduct = await _context.Products.SingleOrDefaultAsync(p => p.Id == product.Id, cancellationToken);

        if (existingProduct is null)
        {
            await AddAsync(product, cancellationToken);

            return;
        }

        product.SearchName = product.Name.ClearToCompare();
        product.UpdatedDate = DateTime.UtcNow;

        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var productToDelete = await _context.Products.SingleOrDefaultAsync(h => h.Id == id, cancellationToken);

        if (productToDelete is not null)
        {
            productToDelete.SetIsDeleted(true);
            _context.Products.Update(productToDelete);
            await _context.SaveChangesAsync(cancellationToken);

            return true;
        }

        return false;
    }

    public async Task<bool> IsThereProductWithNameAsync(string name, Guid? exceptId = null, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return false;
        }

        var nameSearchable = name.ClearToCompare();

        if (exceptId is not null && exceptId != Guid.Empty)
        {
            return await _context.Products.AnyAsync(p => !p.IsDeleted && p.SearchName == nameSearchable && p.Id != exceptId, cancellationToken);
        }

        return await _context.Products.AnyAsync(p => !p.IsDeleted && p.SearchName == nameSearchable, cancellationToken);
    }

    private static IQueryable<Product> SetProductFiltersToQuery(ProductFilter filters, IQueryable<Product> query)
    {
        if (!string.IsNullOrWhiteSpace(filters.Name))
        {
            var nameSearchable = filters.Name.ClearToCompare();
            query = query.Where(p => p.SearchName != null && p.SearchName.Contains(nameSearchable));
        }

        if (!string.IsNullOrWhiteSpace(filters.Description))
        {
            query = query.Where(p => p.Description != null && p.Description.Contains(filters.Description));
        }

        if (filters.MinPrice is not null && filters.MinPrice.Value >= 0)
        {
            query = query.Where(p => p.Price >= filters.MinPrice.Value);
        }

        if (filters.MaxPrice is not null && filters.MaxPrice.Value > 0)
        {
            query = query.Where(p => p.Price <= filters.MaxPrice.Value);
        }

        if (filters.Category is not null && filters.Category != Category.Undefined)
        {
            query = query.Where(p => p.Category == filters.Category);
        }

        if (filters.MinStockQuantity is not null && filters.MinStockQuantity.Value >= 0)
        {
            query = query.Where(p => p.StockQuantity >= filters.MinStockQuantity.Value);
        }

        if (filters.MaxStockQuantity is not null && filters.MaxStockQuantity.Value > 0)
        {
            query = query.Where(p => p.StockQuantity <= filters.MaxStockQuantity.Value);
        }

        return query;
    }
}