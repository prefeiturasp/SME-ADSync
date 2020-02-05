using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.ADSync.Core
{
    public class Comparador : IComparador
    {
        IRepositorio ladoA;
        IRepositorio ladoB;


        public Comparador(IRepositorio ladoA, IRepositorio ladoB)
        {
            this.ladoA = ladoA;
            this.ladoB = ladoB;
        }

        public IEnumerable<ResultadoComparacaoUsuarioDTO> ObterDiferenca(ModoComparacao mode)
        {
            List<ResultadoComparacaoUsuarioDTO> resultado = new List<ResultadoComparacaoUsuarioDTO>();

            switch (mode)
            {
                case ModoComparacao.Total:
                    resultado.AddRange(ObterDiferenca(ladoA, ladoB));
                    resultado.AddRange(ObterDiferenca(ladoB, ladoA, resultado.Where(x => x.Resultado == ResultadoComparacao.AmbosLados).Select(x=> x.LadoA)));
                    break;

                case ModoComparacao.OrientadoPeloLadoA:
                    resultado.AddRange(ObterDiferenca(ladoA, ladoB));
                    break;

                case ModoComparacao.OrientadoPeloLadoB:
                    resultado.AddRange(ObterDiferenca(ladoB, ladoA));
                    break;
            }

            return resultado;
        }

        private IEnumerable<ResultadoComparacaoUsuarioDTO> ObterDiferenca(IRepositorio comparador, IRepositorio comparado, IEnumerable<UsuarioDTO> ignorar = null)
        {
            IEnumerable<UsuarioDTO> ladoA = comparador.ObterParaComparacao().Where(x=> !(ignorar ?? Enumerable.Empty<UsuarioDTO>()).Contains(x));
            IEnumerable<UsuarioDTO> ladoB = null;
            if (ladoA != null && ladoA.Count() > 0)
                ladoB = comparado.Listar(ladoA.Select(x => x.Login).ToArray());

            return ObterResultadoComparacao(ladoA, ladoB);
        }

        private IEnumerable<ResultadoComparacaoUsuarioDTO> ObterResultadoComparacao(IEnumerable<UsuarioDTO> ladoA, IEnumerable<UsuarioDTO> ladoB)
        {
            List<ResultadoComparacaoUsuarioDTO> resultado = new List<ResultadoComparacaoUsuarioDTO>();

            Console.WriteLine($"Processo de massa de dados iniciado - Lado A: {ladoA.Count()} itens, Lado B: {ladoB.Count()} itens");

            //Both sides
            resultado.AddRange(from a in ladoA
                            join b in ladoB
                            on a.Login equals b.Login
                            select new ResultadoComparacaoUsuarioDTO() { LadoA = a, LaboB = b });

            // Only A
            resultado.AddRange(from a in ladoA
                            join b in ladoB
                            on a.Login equals b.Login into g
                            from j in g.DefaultIfEmpty()
                            where j == null
                            select new ResultadoComparacaoUsuarioDTO() { LadoA = a });

            // Only B
            resultado.AddRange(from b in ladoB
                            join a in ladoA
                            on b.Login equals a.Login into g
                            from j in g.DefaultIfEmpty()
                            where j == null
                            select new ResultadoComparacaoUsuarioDTO() { LaboB = b });

            Console.WriteLine($"Processo de massa de dados concluído - {resultado.Count} obtidos");

            return resultado;
        }
    }
}
