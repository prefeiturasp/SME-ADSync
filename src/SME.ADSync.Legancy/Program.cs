using SME.SyncADxCoreSSO.Comandos;
using System;
using System.Configuration;
using System.Linq;

namespace SME.ADSync.Legancy
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Iniciando");

            var connectionString = ConfigurationManager.ConnectionStrings["CoreSSO"].ToString();
            var dominio = ConfigurationManager.AppSettings["domain"];
            var diretorio = ConfigurationManager.AppSettings["container"];
            var usuario = ConfigurationManager.AppSettings["userAD"];
            var senha = ConfigurationManager.AppSettings["passwordAD"];

            Console.WriteLine("Iniciado!");

            IComando comando = null;
            
            Console.WriteLine("Indicar comando [ObterUsuarioAD, ObterUsuarioCoreSSO, Comparar, IncluirUsuariosNoAD, ComparaPorArquivo, ResetarSenhaNoAD]:");
            comando = ObterComando();

            while (comando != null)
            {
                if (comando != null)
                    comando.Executar(connectionString, dominio, diretorio,usuario, senha);

                Console.WriteLine("[Enter] para continuar");
                Console.ReadKey();

                Console.Clear();
                Console.WriteLine("Indicar comando [ObterUsuarioAD, ObterUsuarioCoreSSO, Comparar, IncluirUsuariosNoAD, ComparaPorArquivo, ResetarSenhaNoAD]:");
                comando = ObterComando();
            }
        }

        private static IComando ObterComando()
        {
            IComando comando;
            string comandoStr = Console.ReadLine();
            var tiposComando = AppDomain.CurrentDomain.GetAssemblies().SelectMany(x => x.GetTypes()).Where(x => typeof(IComando).IsAssignableFrom(x));
            var tipo = tiposComando.Where(x => x.Name == $"Comando{comandoStr}").FirstOrDefault();
            if (tipo != null)
            {
                comando = (IComando)Activator.CreateInstance(tiposComando.Where(x => x.Name == $"Comando{comandoStr}").FirstOrDefault());
                return comando;
            }
            return null;
        }
    }
}
