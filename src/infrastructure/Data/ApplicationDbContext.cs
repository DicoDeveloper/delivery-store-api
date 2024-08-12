using System.Reflection;
using Application.Common.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class ApplicationDbContext : DbContext, IApplicationDbContext
{
       public DbSet<Product> Products { get; set; }
       public DbSet<Sale> Sales { get; set; }
       public DbSet<SaleItem> SaleItems { get; set; }
       public DbSet<Company> Companies { get; set; }

       public ApplicationDbContext(
              DbContextOptions<ApplicationDbContext> options
       ) : base(options) { }

       protected override void OnModelCreating(ModelBuilder builder)
       {
              builder.Entity<Product>()
                     .Property(u => u.Id)
                     .HasDefaultValueSql("NEWID()");

              builder.Entity<SaleItem>()
                     .Property(u => u.Id)
                     .HasDefaultValueSql("NEWID()");

              builder.Entity<SaleItem>()
                     .HasOne(s => s.Product)
                     .WithMany()
                     .HasForeignKey(s => s.ProductId)
                     .OnDelete(DeleteBehavior.Restrict);

              builder.Entity<SaleItem>()
                     .HasOne<Sale>()
                     .WithMany(s => s.Items)
                     .HasForeignKey(s => s.SaleId);

              builder.Entity<Sale>()
                     .Property(u => u.Id)
                     .HasDefaultValueSql("NEWID()");

              builder.Entity<Sale>()
                     .HasMany(s => s.Items)
                     .WithOne()
                     .OnDelete(DeleteBehavior.Cascade);

              builder.Entity<Company>()
                     .Property(u => u.Id)
                     .HasDefaultValueSql("NEWID()");

              base.OnModelCreating(builder);
              builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
       }
}