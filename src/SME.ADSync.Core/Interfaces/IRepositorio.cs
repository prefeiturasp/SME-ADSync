using SME.ADSync.Core.DTO;
using System.Collections.Generic;

namespace SME.ADSync.Core.Interfaces
{
    public interface IRepositorio
    {
        bool CriarUsuario(UsuarioDTO user);

        IEnumerable<ResultadoSincronismoDTO> CriarUsuario(IEnumerable<UsuarioDTO> users);

        bool ResetarSenhaParaPadrao(UsuarioDTO user, string conextoLog = "");

        IEnumerable<UsuarioDTO> Listar(string[] logons);

        UsuarioDTO ObterUmOuPadrao(string logon);

        IEnumerable<UsuarioDTO> ObterParaComparacao();

        string ObterSenhaPadrao(string login);
    }
}
