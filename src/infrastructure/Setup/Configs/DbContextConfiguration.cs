using Application.Common.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Setup.Configs;

public static class DbContextConfiguration
{
    public static IServiceCollection AddDbContext(this IServiceCollection service, string stringConnection)
        => service.AddDbContext<ApplicationDbContext>(
                      options => options.UseSqlite(stringConnection).EnableSensitiveDataLogging()
                  )
                  .AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>())
                  .AddScoped<ApplicationDbContextInitialiser>();
}