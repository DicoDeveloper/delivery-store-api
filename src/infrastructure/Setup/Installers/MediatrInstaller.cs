using Application.Common;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Setup.Installers;

public static class MediatrInstaller
{
    public static IServiceCollection InstallMediatr(this IServiceCollection service)
        => service.AddMediatR(typeof(BaseCommandQuery).Assembly);
}