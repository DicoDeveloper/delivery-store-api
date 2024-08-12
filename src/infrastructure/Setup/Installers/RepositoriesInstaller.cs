using Application.Interfaces.Repositories;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Setup.Installers;

public static class RepositoriesInstaller
{
    public static IServiceCollection InstallRepositories(this IServiceCollection service)
        => service.AddScoped<IProductRepository, ProductRepository>()
                  .AddScoped<ISaleRepository, SaleRepository>()
                  .AddScoped<ICompanyRepository, CompanyRepository>();
}