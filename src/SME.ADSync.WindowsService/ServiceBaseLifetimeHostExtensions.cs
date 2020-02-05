using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.ADSync.Worker.Service;
using System.Threading;
using System.Threading.Tasks;

namespace SME.ADSync.WindowsService
{
    public static class ServiceBaseLifetimeHostExtensions
    {
        public static IHostBuilder UseServiceBaseLifetime(this IHostBuilder hostBuilder)
        {
            return hostBuilder
                .ConfigureAppConfiguration((hostContext, config) =>
                {
                    config.AddEnvironmentVariables("ADSync_");
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IHostLifetime, ServiceBaseLifetime>();
                    WorkerService.ConfigurarDependencias(hostContext.Configuration, services, true);
                    WorkerService.Configurar(hostContext.Configuration, services);
                });
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken = default)
        {
            return hostBuilder.UseServiceBaseLifetime().Build().RunAsync(cancellationToken);
        }
    }
}
