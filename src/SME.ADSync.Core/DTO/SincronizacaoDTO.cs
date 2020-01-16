using System;

namespace SME.ADSync.Core.DTO
{
    public class SincronizacaoDTO
    {
        public Guid UsuarioIdCoreSSO { get; set; }
        public DateTime DataUltimaSincronizacao { get; set; }
        public bool Ativo { get; set; }
    }
}
