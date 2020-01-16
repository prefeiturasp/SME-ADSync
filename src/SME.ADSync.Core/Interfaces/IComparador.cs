using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using System.Collections.Generic;

namespace SME.ADSync.Core.Interfaces
{
    public interface IComparador
    {
        IEnumerable<ResultadoComparacaoUsuarioDTO> ObterDiferenca(ModoComparacao mode);
    }
}
