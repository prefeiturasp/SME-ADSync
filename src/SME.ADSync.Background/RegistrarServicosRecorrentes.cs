using Hangfire;
using SME.ADSync.Core;
using SME.ADSync.Core.Interfaces.Servicos;

namespace SME.ADSync.Background
{
    public static class RegistrarServicosRecorrentes
    {
        public static void Registrar()
        {
            // Diariamente as 22:30hrs
            Cliente.ExecutarPeriodicamente<IServicoIncluirUsuariosAD>(c => c.IncluirUsuariosADOrigemCoreSSO(), "0 30 22 * * *");            
            // A cada hora
            Cliente.ExecutarPeriodicamente<IServicoAtualizarUsuariosAD>(c => c.AtualizarUsuariosAD(), "0 0 * * * *");            
        }
    }
}
