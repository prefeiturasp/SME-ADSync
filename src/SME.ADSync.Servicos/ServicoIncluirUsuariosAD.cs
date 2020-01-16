using Newtonsoft.Json;
using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Extesions;
using SME.ADSync.Core.Interfaces;
using SME.ADSync.Core.Interfaces.Servicos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SME.ADSync.Servicos
{
    public class ServicoIncluirUsuariosAD : IServicoIncluirUsuariosAD
    {
        private readonly IComparador comparador;
        private readonly IConsultaOU consultaOU;
        private readonly IRepositorioAD repositorioAD;
        private readonly IRepositorioCoreSSO repositorioCoreSSO;

        public ServicoIncluirUsuariosAD(IComparador comparador,
                                        IConsultaOU consultaOU,
                                        IRepositorioAD repositorioAD,
                                        IRepositorioCoreSSO repositorioCoreSSO)
        {
            this.comparador = comparador ?? throw new ArgumentNullException(nameof(comparador));
            this.consultaOU = consultaOU ?? throw new ArgumentNullException(nameof(consultaOU));
            this.repositorioAD = repositorioAD ?? throw new ArgumentNullException(nameof(repositorioAD));
            this.repositorioCoreSSO = repositorioCoreSSO ?? throw new ArgumentNullException(nameof(repositorioCoreSSO));
        }

        public void IncluirUsuariosADOrigemCoreSSO()
        {
            var registrosInclusao = from i in comparador.ObterDiferenca(ModoComparacao.OrientadoPeloLadoA)
                                    where i.LadoA != null && i.Resultado == ResultadoComparacao.SomenteLadoA
                                    select i.LadoA;

            IList<ResultadoImportacaoDTO> resultados = new List<ResultadoImportacaoDTO>();

            foreach (var item in registrosInclusao)
            {
                ResultadoImportacaoDTO resultado = new ResultadoImportacaoDTO() { Usuario = item };

                try
                {
                    var ouUsuario = consultaOU.MontarOUUsuario(item.Login, item.OU);

                    if ((TipoCriptografia)item.Criptografia != TipoCriptografia.TripleDES)
                        repositorioCoreSSO.ResetarSenhaParaPadrao(item);

                    if (!string.IsNullOrWhiteSpace(ouUsuario))
                        resultado.Resultado = repositorioAD.CriarUsuario(item) ? ResultadoImportacao.Sucesso : ResultadoImportacao.FalhaNaoIdentificada;
                    else
                        resultado.Resultado = ResultadoImportacao.NaoFoiPossivelIdentificarOU;
                }
                catch (Exception ex)
                {
                    resultado.Resultado = ResultadoImportacao.Erro;
                    resultado.MensagemErro = ex.ToString();
                }

                resultados.Add(resultado);
            }

            Log.GravarArquivo(JsonConvert.SerializeObject(resultados), "IncluirUsuariosADOrigemCoreSSO");
        }
    }
}
