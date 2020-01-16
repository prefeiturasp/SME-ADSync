using Newtonsoft.Json;
using SME.ADSync.Core;
using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Interfaces;
using SME.SyncADxCoreSSO.Repositorios;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SME.SyncADxCoreSSO.Comandos
{
    public class ComandoComparar : IComando
    {
        internal IEnumerable<ResultadoComparacaoUsuarioDTO> Comparar(string connectionString, string dominio, string diretorio, string usuario, string senha, ModoComparacao tipo)
        {
            IRepositorio ladoA = null;
            IRepositorio ladoB = null;

            ConstruirLados(connectionString, dominio, diretorio, usuario, senha, tipo, ref ladoA, ref ladoB);

            if (ladoA != null && ladoB != null)
                return new Comparador(ladoA, ladoB).ObterDiferenca(tipo);
            else
                return Enumerable.Empty<ResultadoComparacaoUsuarioDTO>();
        }

        public void Executar(params string[] args)
        {
            var connectionString = args[0];
            var dominio = args[1];
            var diretorio = args[2];
            var usuario = args[3];
            var senha = args[4];

            Console.WriteLine("Tipo de comparacao [Total, OrientadoPeloLadoA, OrientadoPeloLadoB]?");
            ModoComparacao tipo;

            if (Enum.TryParse(Console.ReadLine(), false, out tipo))
            {
                string json = JsonConvert.SerializeObject(Comparar(connectionString, dominio, diretorio, usuario, senha, tipo));
                File.WriteAllText($"c:\\jsonAd\\arquivo_{DateTime.Now.ToString("yyyyMMddHHmmss")}.json", json);
            }
        }

        protected virtual void ConstruirLados(string connectionString, string dominio, string diretorio, string usuario, string senha, ModoComparacao tipo, ref IRepositorio ladoA, ref IRepositorio ladoB)
        {
            if (tipo != ModoComparacao.Total)
            {
                Console.WriteLine("Qual o lado A [AD, CoreSSO]?");
                var lado = Console.ReadLine();

                if (lado == "AD")
                {
                    ladoA = new ADSync.SMEADSync(dominio, diretorio, usuario, senha);
                    ladoB = new RepositorioCoreSSO(connectionString);
                }
                else if (lado == "CoreSSO")
                {
                    ladoA = new RepositorioCoreSSO(connectionString);
                    ladoB = new ADSync.SMEADSync(dominio, diretorio, usuario, senha);
                }
            }
            else
            {
                ladoA = new RepositorioCoreSSO(connectionString);
                ladoB = new ADSync.SMEADSync(dominio, diretorio, usuario, senha);
            }
        }
    }
}
