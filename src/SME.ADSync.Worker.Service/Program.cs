using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace SME.ADSync.Worker.Service
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var asService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()  
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

            builder.UseEnvironment(asService ? Environments.Production : Environments.Development);

            if (asService)
                await builder.Build().RunAsync();
            else
                await builder.RunConsoleAsync();
        }
    }
}
