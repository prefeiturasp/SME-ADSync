using SME.ADSync.Core.DTO;
using System;

namespace SME.ADSync.Core.Interfaces
{
    public interface IRepositorioADSync
    {
        void IncluirSincronizacao(SincronizacaoDTO sincronizacao);
        SincronizacaoDTO ObterSincronizacao(Guid usuarioIdCoreSSO);
        void AtualizarSincronizacao(SincronizacaoDTO sincronizacao);
    }
}
