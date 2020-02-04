using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.ADSync.Worker.Service;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SME.ADSync.WindowsService
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var asService = !(Debugger.IsAttached || args.Contains("--console"));

            IHostBuilder builder;

            if (asService)
            {
                builder = new HostBuilder()
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<WorkerService>();
                    });
            }
            else
            {
                builder = new HostBuilder()
                    .ConfigureAppConfiguration((hostContext, config) =>
                    {
                        config.AddEnvironmentVariables("ADSync_");
                    })
                    .ConfigureServices((hostContext, services) =>
                    {
                        services.AddHostedService<WorkerService>();
                        WorkerService.ConfigurarDependencias(hostContext.Configuration, services);
                        WorkerService.Configurar(hostContext.Configuration, services);
                    });
            }

            builder.UseEnvironment(asService ? Environments.Production : Environments.Development);

            if (asService)
                await builder.RunAsServiceAsync();
            else
                await builder.RunConsoleAsync();
        }
    }
}
