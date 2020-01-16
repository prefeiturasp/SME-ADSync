using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SME.ADSync.Core;
using SME.ADSync.Core.Interfaces;
using SME.ADSync.Core.Interfaces.Servicos;
using SME.ADSync.Infra.Contexto;
using SME.ADSync.Infra.Interfaces;
using SME.ADSync.IoC.Extensions;
using SME.ADSync.Servicos;
using SME.SyncADxCoreSSO.ADSync;
using SME.SyncADxCoreSSO.Repositorios;

namespace SME.ADSync.IoC
{
    public static class RegistraDependencias
    {

        public static void Registrar(IServiceCollection services, IConfiguration configuration)
        {
            RegistrarContextos(services);
            RegistrarRepositorios(services, configuration);
            RegistrarServicos(services);
        }

        private static void RegistrarContextos(IServiceCollection services)
        {
            services.TryAddScopedWorkerService<IContextoAplicacao, WorkerContext>();            
        }

        private static void RegistrarRepositorios(IServiceCollection services, IConfiguration configuration)
        {
            var repositorioCoreSSO = new RepositorioCoreSSO(configuration.GetConnectionString("CoreSSO"));
            var repositorioAD = new SMEADSync(configuration["domain"], 
                                              configuration["container"], 
                                              configuration["userAD"], 
                                              configuration["passwordAD"]);

            services.TryAddScopedWorkerService<IRepositorioCoreSSO>(_ => repositorioCoreSSO);
            services.TryAddScopedWorkerService<IRepositorioAD>(_ => repositorioAD);
            services.TryAddScopedWorkerService<IComparador>(_ => new Comparador(repositorioCoreSSO, repositorioAD));
            services.TryAddScopedWorkerService<IConsultaOU>(_ => repositorioCoreSSO);
        }

        private static void RegistrarServicos(IServiceCollection services)
        {
            services.TryAddScopedWorkerService<IServicoIncluirUsuariosAD, ServicoIncluirUsuariosAD>();
        }
    }
}
