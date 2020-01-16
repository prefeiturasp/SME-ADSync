using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SME.ADSync.Core.Enumerados;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ADSync.Core.DTO
{
    public class ResultadoComparacaoUsuarioDTO
    {
        public UsuarioDTO LadoA { get; set; }
        public UsuarioDTO LaboB { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultadoComparacao Resultado
        {
            get
            {
                if (LadoA != null && LaboB != null && LadoA == LaboB)
                    return ResultadoComparacao.AmbosLados;
                else if (LadoA != null)
                    return ResultadoComparacao.SomenteLadoA;
                else
                    return ResultadoComparacao.SomenteLadoB;
            }
        }

    }
}
