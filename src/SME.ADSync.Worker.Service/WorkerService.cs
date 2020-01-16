using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SME.ADSync.Background;
using SME.ADSync.Core;
using SME.ADSync.HangFire;
using SME.ADSync.IoC;

namespace SME.ADSync.Worker.Service
{
    public class WorkerService : IHostedService
    {
        private static Servidor<HangFire.Worker> HangfireWorkerService;
        private string ipLocal;

        protected string IPLocal
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ipLocal))
                {
                    var host = Dns.GetHostEntry(Dns.GetHostName());
                    foreach (var ip in host.AddressList)
                    {
                        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                            ipLocal = ip.ToString();

                    }
                    if (string.IsNullOrWhiteSpace(ipLocal))
                        ipLocal = "127.0.0.1";
                }

                return ipLocal;
            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            HangfireWorkerService.Registrar();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            HangfireWorkerService.Dispose();
            return Task.CompletedTask;
        }

        internal static void Configurar(IConfiguration config, IServiceCollection services)
        {
            HangfireWorkerService = new Servidor<HangFire.Worker>(new HangFire.Worker(config, services, config.GetConnectionString("ADSync-HangFire-SqlServer")));
        }

        internal static void ConfigurarDependencias(IConfiguration configuration, IServiceCollection services)
        {
            RegistraDependencias.Registrar(services, configuration);
            Orquestrador.Inicializar(services.BuildServiceProvider());
            Orquestrador.Registrar(new Processor(configuration, "ADSync-HangFire-SqlServer"));
            RegistrarServicosRecorrentes.Registrar();
        }
    }
}
