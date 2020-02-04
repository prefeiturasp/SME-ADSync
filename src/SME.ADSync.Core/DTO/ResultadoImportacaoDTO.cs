using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SME.ADSync.Core.Enumerados;

namespace SME.ADSync.Core.DTO
{
    public class ResultadoImportacaoDTO : ResultadoBaseDTO
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultadoImportacao Resultado { get; set; }
    }
}
