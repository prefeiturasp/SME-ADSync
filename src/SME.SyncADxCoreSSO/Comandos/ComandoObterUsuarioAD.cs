using SME.SyncADxCoreSSO.ADSync;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.SyncADxCoreSSO.Comandos
{
    public class ComandoObterUsuarioAD : IComando
    {
        public void Executar(params string[] args)
        {
            var dominio = args[1];
            var diretorio = args[2];
            var usuarioAD = args[3];
            var senha = args[4];

            Console.WriteLine("Qual usuario?");
            var usuario = Console.ReadLine();
            var resultado = new SMEADSync(dominio, diretorio, usuarioAD, senha).ObterUmOuPadrao(usuario);
            Console.WriteLine(resultado != null ? resultado.ToString() : "nulo");
        }
    }
}
