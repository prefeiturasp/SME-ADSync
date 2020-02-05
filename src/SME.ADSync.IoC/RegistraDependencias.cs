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

        public static void Registrar(IServiceCollection services, IConfiguration configuration, bool singleton = false)
        {
            RegistrarContextos(services, singleton);
            RegistrarRepositorios(services, configuration, singleton);
            RegistrarServicos(services, singleton);
        }

        private static void RegistrarContextos(IServiceCollection services, bool singleton)
        {
            if (singleton)
                services.AddSingleton<IContextoAplicacao, WorkerContext>();
            else
                services.TryAddScopedWorkerService<IContextoAplicacao, WorkerContext>();
        }

        private static void RegistrarRepositorios(IServiceCollection services, IConfiguration configuration, bool singleton)
        {
            var repositorioADSync = new RepositorioADSync(configuration.GetConnectionString("ADSync-SqlServer"));
            var repositorioCoreSSO = new RepositorioCoreSSO(configuration.GetConnectionString("CoreSSO"));
            var repositorioAD = new SMEADSync(configuration["domain"],
                                              configuration["container"],
                                              configuration["userAD"],
                                              configuration["passwordAD"]);

            if (singleton)
            {
                services.AddSingleton<IRepositorioADSync>(_ => repositorioADSync);
                services.AddSingleton<IRepositorioCoreSSO>(_ => repositorioCoreSSO);
                services.AddSingleton<IRepositorioAD>(_ => repositorioAD);
                services.AddSingleton<IComparador>(_ => new Comparador(repositorioCoreSSO, repositorioAD));
                services.AddSingleton<IConsultaOU>(_ => repositorioCoreSSO);
            }
            else
            {
                services.TryAddScopedWorkerService<IRepositorioADSync>(_ => repositorioADSync);
                services.TryAddScopedWorkerService<IRepositorioCoreSSO>(_ => repositorioCoreSSO);
                services.TryAddScopedWorkerService<IRepositorioAD>(_ => repositorioAD);
                services.TryAddScopedWorkerService<IComparador>(_ => new Comparador(repositorioCoreSSO, repositorioAD));
                services.TryAddScopedWorkerService<IConsultaOU>(_ => repositorioCoreSSO);
            }
        }

        private static void RegistrarServicos(IServiceCollection services, bool singleton)
        {
            if (singleton)
            {
                services.AddSingleton<IServicoIncluirUsuariosAD, ServicoIncluirUsuariosAD>();
                services.AddSingleton<IServicoAtualizarUsuariosAD, ServicoAtualizarUsuariosAD>();
            }
            else
            {
                services.TryAddScopedWorkerService<IServicoIncluirUsuariosAD, ServicoIncluirUsuariosAD>();
                services.TryAddScopedWorkerService<IServicoAtualizarUsuariosAD, ServicoAtualizarUsuariosAD>();
            }
        }
    }
}
