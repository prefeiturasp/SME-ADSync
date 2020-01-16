using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using SME.ADSync.Core.Enumerados;
using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ADSync.Core.DTO
{
    public class ResultadoImportacaoDTO
    {
        public UsuarioDTO Usuario { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public ResultadoImportacao Resultado { get; set; }
        public string MensagemErro { get; set; }
    }
}
