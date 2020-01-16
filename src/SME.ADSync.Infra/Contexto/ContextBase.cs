using SME.ADSync.Infra.Interfaces;
using System.Collections.Generic;

namespace SME.ADSync.Infra.Contexto
{
    public abstract class ContextBase : IContextoAplicacao
    {
        public ContextBase()
        {
            Variaveis = new Dictionary<string, object>();
        }

        public IDictionary<string, object> Variaveis { get; set; }
        public string UsuarioLogado => ObterVariavel<string>("NomeUsuario") ?? "Sistema";
        public string NomeUsuario => ObterVariavel<string>("UsuarioLogado") ?? "Sistema";

        public abstract IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto);

        public T ObterVariavel<T>(string nome)
        {
            object valor = null;

            if (Variaveis.TryGetValue(nome, out valor))
                return (T)valor;

            return default(T);
        }
    }
}
