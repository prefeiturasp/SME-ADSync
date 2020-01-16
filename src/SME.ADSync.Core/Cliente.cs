using SME.ADSync.Core.Enumerados;
using System;
using System.Linq.Expressions;

namespace SME.ADSync.Core
{
    public static class Cliente
    {
        public static string Executar(Expression<Action> metodo, TipoProcessamento tipoProcessamento = TipoProcessamento.ExecucaoImediata)
        {
            GravarLog($"Novo processamento background solicitado {metodo.Body.ToString()}");
            return Orquestrador
                .ObterProcessador(tipoProcessamento)
                .Executar(metodo);
        }

        public static string Executar<T>(Expression<Action<T>> metodo, TipoProcessamento tipoProcessamento = TipoProcessamento.ExecucaoImediata)
        {
            GravarLog($"Novo processamento background solicitado {metodo.Body.ToString()}");
            return Orquestrador
                .ObterProcessador(tipoProcessamento)
                .Executar<T>(metodo);
        }

        public static void ExecutarPeriodicamente<T>(Expression<Action<T>> metodo, string cron)
        {
            Orquestrador
                .ObterProcessador(TipoProcessamento.ExecucaoRecorrente)
                .ExecutarPeriodicamente(metodo, cron);
        }

        private static void GravarLog(string mensagem)
        {            
            Console.WriteLine($"{mensagem} - {DateTime.Now.ToString("MM/dd/yyyy hh:mm:ss.fff tt")}");
        }
    }
}
