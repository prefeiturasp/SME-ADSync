using SME.ADSync.Core.Enumerados;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using SME.ADSync.Core.DTO;
using Newtonsoft.Json;
using SME.ADSync.Core.Interfaces;
using SME.SyncADxCoreSSO.Repositorios;
using System.Diagnostics;

namespace SME.SyncADxCoreSSO.Comandos
{
    public class ComandoResetarSenhaNoAD : IComando
    {
        public void Executar(params string[] args)
        {
            IEnumerable<UsuarioDTO> paraIncluir = null;
            IEnumerable<ResultadoImportacaoDTO> resultado = null;

            var connectionString = args[0];
            var dominio = args[1];
            var diretorio = args[2];
            var usuario = args[3];
            var senha = args[4];

            IConsultaOU repositorioConsultaOU = new RepositorioCoreSSO(connectionString);

            paraIncluir = ObterDadosPorArquivo();

            if (paraIncluir != null && paraIncluir.Count() > 0)
                resultado = ResetarSenha(paraIncluir, dominio, diretorio, usuario, senha, repositorioConsultaOU);

            if (resultado != null)
            {
                string json = JsonConvert.SerializeObject(resultado);
                SME.ADSync.Core.Extesions.Log.GravarArquivo(json, "ResetarSenhaNoAD_");
            }
        }

        private IEnumerable<ResultadoImportacaoDTO> ResetarSenha(IEnumerable<UsuarioDTO> paraIncluir, string dominio, string diretorio, string usuario, string senha, IConsultaOU ouDestino)
        {
            List<ResultadoImportacaoDTO> resultados = new List<ResultadoImportacaoDTO>();

            Console.WriteLine($"Confirma a alteração de senha de {paraIncluir.Count()} confirma [Sim, Nao]?");
            var confirmacao = Console.ReadLine();
            if (confirmacao == "Sim")
            {
                Stopwatch crono = Stopwatch.StartNew();
                int i = 0;
                string contextoLog = Guid.NewGuid().ToString();
                foreach (var item in paraIncluir)
                {
                    ResultadoImportacaoDTO resultado = new ResultadoImportacaoDTO() { Usuario = item };

                    try
                    {
                        var repositorio = new SME.SyncADxCoreSSO.ADSync.SMEADSync(dominio, diretorio, usuario, senha);

                        resultado.Resultado = repositorio.ResetarSenhaParaPadrao(item, contextoLog) ? ResultadoImportacao.Sucesso : ResultadoImportacao.FalhaNaoIdentificada;

                    }
                    catch (Exception ex)
                    {
                        resultado.Resultado = ResultadoImportacao.Erro;
                        resultado.MensagemErro = ex.ToString();
                    }

                    resultados.Add(resultado);
                    i++;

                    if (i % 100 == 0)
                    {
                        Console.WriteLine($"Foram processados {i} itens - {crono.ElapsedMilliseconds / 1000} segundos");
                        crono.Restart();
                    }
                }
            }

            return resultados;
        }

        private IEnumerable<UsuarioDTO> ObterDadosPorArquivo()
        {
            List<UsuarioDTO> retorno = new List<UsuarioDTO>();

            Console.WriteLine("Favor informar o caminho do arquivo (separado por vírgula)");
            var caminho = Console.ReadLine();

            if (File.Exists(caminho))
            {
                var dados = File.ReadAllLines(caminho);

                for (int i = 1; i < dados.Length; i++)
                {
                    if (dados[i].Contains(";"))
                    {
                        var usuarioStr = dados[i].Split(';');

                        retorno.Add(new UsuarioDTO()
                        {
                            PrimeiroNome = usuarioStr[0],
                            Sobrenome = usuarioStr[1],
                            Login = usuarioStr[2],
                            Email = usuarioStr.Length >= 4 ? usuarioStr[3] : null,
                            OU = usuarioStr.Length >= 5 ? usuarioStr[4] : null,
                            Descricao = usuarioStr.Length >= 6 ? usuarioStr[5] : null,
                        });
                    }
                    else
                        Console.WriteLine($"A linha {i + 1} não atende aos requisitos para processamento - {dados[i]}");
                }
            }
            else
                Console.WriteLine("Arquivo não encontrado");

            return retorno;
        }
    }
}
