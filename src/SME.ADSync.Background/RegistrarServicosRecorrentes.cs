using Hangfire;
using SME.ADSync.Core;
using SME.ADSync.Core.Interfaces.Servicos;

namespace SME.ADSync.Background
{
    public static class RegistrarServicosRecorrentes
    {
        public static void Registrar()
        {
            Cliente.ExecutarPeriodicamente<IServicoIncluirUsuariosAD>(c => c.IncluirUsuariosADOrigemCoreSSO(), Cron.MinuteInterval(5));
        }
    }
}
