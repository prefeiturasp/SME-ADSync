﻿using System;
using System.Linq.Expressions;
using SME.ADSync.Core.Interfaces;

namespace SME.ADSync.Core.Processors
{
    public class DisabledProcessor : IProcessor
    {
        public bool Registrado => true;

        public string Executar(Expression<Action> metodo)
        {
            var acao = metodo.Compile();
            acao.Invoke();
            return string.Empty;
        }

        public string Executar<T>(Expression<Action<T>> metodo)
        {
            var classe = (T)Orquestrador.Provider.GetService(typeof(T));
            var acao = metodo.Compile();
            acao(classe);
            return string.Empty;
        }

        public void ExecutarPeriodicamente(Expression<Action> metodo, string cron)
        {
            throw new Exception("Não é possível realizar novos processamentos periódicos pois o serviço de processamento em segundo plano está desativado");
        }

        public void ExecutarPeriodicamente<T>(Expression<Action<T>> metodo, string cron)
        {
            throw new Exception("O serviço de processamento em segundo plano está desativado");
        }

        public void Registrar()
        {            
        }
    }
}
