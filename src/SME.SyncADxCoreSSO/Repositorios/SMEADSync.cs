using SME.ADSync.Core;
using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SME.SyncADxCoreSSO.ADSync
{
    public class SMEADSync : RepositorioADBase
    {
        public SMEADSync(string dominio, string diretorio, string usuario, string senha)
            : base(dominio, diretorio, usuario, senha) { }

        public override IEnumerable<UsuarioDTO> ObterParaComparacao()
        {
            return Enumerable.Empty<UsuarioDTO>();
        }

        public override string ObterSenhaPadrao(string login)
        {
            return $"Sei@{login.Substring(login.Length - 4, 4)}";
        }
    }
}
