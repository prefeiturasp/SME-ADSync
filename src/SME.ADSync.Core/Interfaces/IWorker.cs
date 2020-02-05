using System;

namespace SME.ADSync.Core.Interfaces
{
    public interface IWorker : IDisposable
    {
        void Registrar();
    }
}
