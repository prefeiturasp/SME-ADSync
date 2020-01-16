﻿using System;
using System.Linq.Expressions;

namespace SME.ADSync.Core.Interfaces
{
    public interface IProcessor
    {
        bool Registrado { get; }
        string Executar(Expression<Action> metodo);
        string Executar<T>(Expression<Action<T>> metodo);
        void ExecutarPeriodicamente(Expression<Action> metodo, string cron);
        void ExecutarPeriodicamente<T>(Expression<Action<T>> metodo, string cron);
        void Registrar();
    }
}
