using SME.ADSync.Core.Interfaces;
using System;

namespace SME.ADSync.Core
{
    public class Servidor<T> : IDisposable
        where T : IWorker
    {
        private readonly T worker;

        public Servidor(T worker)
        {
            this.worker = worker;
        }

        public void Dispose()
        {
            worker?.Dispose();
        }

        public void Registrar()
        {
            worker.Registrar();
        }
    }
}
