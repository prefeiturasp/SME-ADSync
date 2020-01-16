using SME.ADSync.Infra.Escopo;
using SME.ADSync.Infra.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading;

namespace SME.ADSync.Infra.Contexto
{
    public class WorkerContext : ContextBase, IDisposable
    {
        public WorkerContext()
        {
            Variaveis = new Dictionary<string, object>();
            WorkerContext contextoTransiente;        
            
            if(!string.IsNullOrWhiteSpace(WorkerContext.ContextIdentifier) &&
               WorkerServiceScope.TransientContexts.TryGetValue(WorkerContext.ContextIdentifier, out contextoTransiente))
            {
                AtribuirContexto(contextoTransiente);
            }
        }

        public override IContextoAplicacao AtribuirContexto(IContextoAplicacao contexto)
        {
            Variaveis = contexto.Variaveis;
            return this;
        }

        public static string ContextIdentifier
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId.ToString();
            }
        }

        public void Dispose()
        {
            Variaveis.Clear();
        }
    }
}
