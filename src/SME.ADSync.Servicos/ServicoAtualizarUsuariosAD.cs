using Newtonsoft.Json;
using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Extesions;
using SME.ADSync.Core.Interfaces;
using SME.ADSync.Core.Interfaces.Servicos;
using System;
using System.Collections.Generic;

namespace SME.ADSync.Servicos
{
    public class ServicoAtualizarUsuariosAD : IServicoAtualizarUsuariosAD
    {
        private readonly IRepositorioCoreSSO repositorioCoreSSO;
        private readonly IRepositorioADSync repositorioADSync;
        private readonly IRepositorioAD repositorioAD;

        public ServicoAtualizarUsuariosAD(IRepositorioCoreSSO repositorioCoreSSO,
                                          IRepositorioADSync repositorioADSync,
                                          IRepositorioAD repositorioAD)
        {
            this.repositorioCoreSSO = repositorioCoreSSO ?? throw new ArgumentNullException(nameof(repositorioCoreSSO));
            this.repositorioADSync = repositorioADSync ?? throw new ArgumentNullException(nameof(repositorioADSync));
            this.repositorioAD = repositorioAD ?? throw new ArgumentNullException(nameof(repositorioAD));
        }

        public void AtualizarUsuariosAD()
        {
            IList<ResultadoSincronismoDTO> resultados = new List<ResultadoSincronismoDTO>();
            var usuarios = repositorioCoreSSO.ObterParaComparacao();

            foreach (var usuario in usuarios)
            {
                var sincronizacao = repositorioADSync.ObterSincronizacao(usuario.Id);
                if (sincronizacao == null || usuario.DataAlteracao > sincronizacao.DataUltimaSincronizacao)
                {
                    var resultado = new ResultadoSincronismoDTO() { Usuario = usuario };
                    try
                    {
                        if (usuario.Situacao.Equals(3))
                            repositorioAD.DesativarUsuario(usuario.Login);
                        else if ((TipoCriptografia)usuario.Criptografia != TipoCriptografia.TripleDES)
                        {
                            repositorioCoreSSO.ResetarSenhaParaPadrao(usuario);
                            repositorioAD.ResetarSenhaParaPadrao(usuario);
                        }
                        else
                            repositorioAD.AtualizarSenha(usuario.Login, new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES).Decrypt(usuario.Senha));

                        if (sincronizacao == null)
                        {                            
                            repositorioADSync.IncluirSincronizacao(new SincronizacaoDTO()
                            {
                                UsuarioIdCoreSSO = usuario.Id,
                                DataUltimaSincronizacao = DateTime.Now,
                                Ativo = !usuario.Situacao.Equals(3)
                            });
                        }
                        else
                        {
                            sincronizacao.DataUltimaSincronizacao = DateTime.Now;
                            repositorioADSync.AtualizarSincronizacao(sincronizacao);
                        }

                        resultado.Sucesso = true;
                    }
                    catch (Exception ex)
                    {
                        resultado.Sucesso = false;
                        resultado.MensagemErro = ex.Message;
                    }
                    resultados.Add(resultado);
                }
            }
            Log.GravarArquivo(JsonConvert.SerializeObject(resultados), "AtualizarUsuariosAD");
        }
    }
}
