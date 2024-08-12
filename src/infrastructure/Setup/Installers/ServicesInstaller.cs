using Application.ViaCep.Services;
using Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Setup.Installers;

public static class ServicesInstaller
{
    public static IServiceCollection InstallServices(this IServiceCollection service)
        => service.AddScoped<IViaCepApiService, ViaCepApiService>();
}