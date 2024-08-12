using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Common.Interfaces;

public interface IApplicationDbContext : IDisposable
{
    DbSet<Product> Products { get; }
    DbSet<Sale> Sales { get; set; }
    DbSet<SaleItem> SaleItems { get; set; }
    DbSet<Company> Companies { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}