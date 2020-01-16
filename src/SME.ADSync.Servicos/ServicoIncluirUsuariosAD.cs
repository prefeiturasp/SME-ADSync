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
        private readonly IRepositorioADSync repositorioADSync;

        public ServicoIncluirUsuariosAD(IComparador comparador,
                                        IConsultaOU consultaOU,
                                        IRepositorioAD repositorioAD,
                                        IRepositorioCoreSSO repositorioCoreSSO,
                                        IRepositorioADSync repositorioADSync)
        {
            this.comparador = comparador ?? throw new ArgumentNullException(nameof(comparador));
            this.consultaOU = consultaOU ?? throw new ArgumentNullException(nameof(consultaOU));
            this.repositorioAD = repositorioAD ?? throw new ArgumentNullException(nameof(repositorioAD));
            this.repositorioCoreSSO = repositorioCoreSSO ?? throw new ArgumentNullException(nameof(repositorioCoreSSO));
            this.repositorioADSync = repositorioADSync ?? throw new ArgumentNullException(nameof(repositorioADSync));
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

                    AtualizarSincronizacao(item);
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

        private void AtualizarSincronizacao(UsuarioDTO item)
        {
            var sincronizacaoDto = new SincronizacaoDTO()
            {
                UsuarioIdCoreSSO = item.Id,
                DataUltimaSincronizacao = DateTime.Now,
                Ativo = item.Situacao.Equals(3)
            };

            if (repositorioADSync.ObterSincronizacao(item.Id) != null)
                repositorioADSync.AtualizarSincronizacao(sincronizacaoDto);
            else
                repositorioADSync.IncluirSincronizacao(sincronizacaoDto);
        }
    }
}
