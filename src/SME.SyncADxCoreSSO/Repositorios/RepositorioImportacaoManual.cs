using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SME.SyncADxCoreSSO.Repositorios
{
    public class RepositorioImportacaoManual : IRepositorio
    {
        string caminhoArquivo;

        public RepositorioImportacaoManual(string caminhoArquivo)
        {
            this.caminhoArquivo = caminhoArquivo;
        }

        public IEnumerable<UsuarioDTO> ObterParaComparacao()
        {
            List<ImportacaoManualDTO> oqImportar = new List<ImportacaoManualDTO>();

            if (File.Exists(caminhoArquivo))
            {
                var dados = File.ReadAllLines(caminhoArquivo);

                for (int i = 1; i < dados.Length; i++)
                {
                    if (dados[i].Contains(";"))
                    {
                        var usuarioStr = dados[i].Split(';');

                        oqImportar.Add(new ImportacaoManualDTO()
                        {
                            Nome = usuarioStr[0],
                            PrimeiroNome = usuarioStr[1],
                            Sobrenome = usuarioStr[2],
                            RF = usuarioStr[3],
                            RFTratado = usuarioStr[4],
                            Descricao = usuarioStr[5],
                            OU = usuarioStr[6]
                        });
                    }
                    else
                        Console.WriteLine($"A linha {i + 1} não atende aos requisitos para processamento - {dados[i]}");
                }
            }
            else
                Console.WriteLine("Arquivo não encontrado");

            return oqImportar.Select(x => new UsuarioDTO()
            {
                PrimeiroNome = x.PrimeiroNome,
                Sobrenome = x.Sobrenome,
                Login = x.RFTratado,
                OU = x.OU
            });
        }

        public bool CriarUsuario(UsuarioDTO user)
        {
            return false;
        }

        public IEnumerable<ResultadoSincronismoDTO> CriarUsuario(IEnumerable<UsuarioDTO> users)
        {
            return Enumerable.Empty<ResultadoSincronismoDTO>();
        }

        public IEnumerable<UsuarioDTO> Listar(string[] logons)
        {
            return Enumerable.Empty<UsuarioDTO>();
        }

        public UsuarioDTO ObterUmOuPadrao(string logon)
        {
            return null;
        }

        public bool ResetarSenhaParaPadrao(UsuarioDTO user, string conextoLog = "")
        {
            throw new NotImplementedException();
        }

        public string ObterSenhaPadrao(string login)
        {
            throw new NotImplementedException();
        }
    }
}
