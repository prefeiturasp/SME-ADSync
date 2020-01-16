using Newtonsoft.Json;
using SME.ADSync.Core;
using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Interfaces;
using SME.SyncADxCoreSSO.Repositorios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SME.SyncADxCoreSSO.Comandos
{
    public class ComandoCompararPorArquivo : ComandoComparar, IComando
    {
        new public void Executar(params string[] args)
        {
            var connectionString = args[0];
            var dominio = args[1];
            var diretorio = args[2];
            var usuario = args[3];
            var senha = args[4];

            string json = JsonConvert.SerializeObject(Comparar(connectionString, dominio, diretorio, usuario, senha, ModoComparacao.OrientadoPeloLadoA));
            File.WriteAllText($"c:\\jsonAd\\arquivo_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json", json);
        }

        protected override void ConstruirLados(string connectionString, string dominio, string diretorio, string usuario, string senha, ModoComparacao tipo, ref IRepositorio ladoA, ref IRepositorio ladoB)
        {
            Console.WriteLine("Informar o arquivo base para comparação");
            var caminho = Console.ReadLine();

            List<ImportacaoManualDTO> oqImportar = new List<ImportacaoManualDTO>();

            if (File.Exists(caminho))
            {
                ladoA = new RepositorioImportacaoManual(caminho);
                ladoB = new ADSync.SMEADSync(dominio, diretorio, usuario, senha);
            }
            else
                Console.WriteLine("Arquivo não encontrado");
        }
    }
}
