using System.Collections.Generic;

namespace SME.ADSync.Infra.Interfaces
{
    public interface IContextoAplicacao
    {
        IDictionary<string, object> Variaveis { get; set; }
        string UsuarioLogado { get;  }
        string NomeUsuario { get; }
        T ObterVariavel<T>(string nome);
        IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto);
    }
}
