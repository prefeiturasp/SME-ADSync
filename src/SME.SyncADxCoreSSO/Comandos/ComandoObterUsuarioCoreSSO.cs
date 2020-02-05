using SME.SyncADxCoreSSO.Repositorios;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SyncADxCoreSSO.Comandos
{
    public class ComandoObterUsuarioCoreSSO : IComando
    {
        public void Executar(params string[] args)
        {
            var connectionString = args[0];

            Console.WriteLine("Qual usuario?");
            var usuario = Console.ReadLine();
            var resultado = new RepositorioCoreSSO(connectionString).ObterUmOuPadrao(usuario);
            Console.WriteLine(resultado != null ? resultado.ToString() : "nulo");
        }
    }
}
