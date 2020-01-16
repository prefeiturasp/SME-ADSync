using Hangfire.Client;
using Hangfire.Common;
using Hangfire.Server;
using SME.ADSync.Core;
using SME.ADSync.Infra.Contexto;
using SME.ADSync.Infra.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using SME.ADSync.Infra.Escopo;

namespace SME.ADSync.Background
{
    public class ContextFilterAttribute : JobFilterAttribute, IClientFilter, IServerFilter
    {
        public void OnCreated(CreatedContext filterContext)
        {            
        }

        public void OnCreating(CreatingContext filterContext)
        {
            IContextoAplicacao contexto = ObterContexto();
            if (contexto != null)
            {
                var contextoTransiente = new WorkerContext();
                contextoTransiente.AtribuirContexto(contexto);
                filterContext.SetJobParameter("contextoAplicacao", contextoTransiente);
            }
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            WorkerServiceScope.DestroyScope();
        }

        public void OnPerforming(PerformingContext filterContext)
        {
            var contextoTransiente = filterContext.GetJobParameter<WorkerContext>("contextoAplicacao");
            WorkerServiceScope.TransientContexts.TryAdd(WorkerContext.ContextIdentifier, contextoTransiente);
        }

        private IContextoAplicacao ObterContexto()
        {
            var provider = Orquestrador.Provider;
            return provider.GetService<IContextoAplicacao>();            
        }
    }
}
