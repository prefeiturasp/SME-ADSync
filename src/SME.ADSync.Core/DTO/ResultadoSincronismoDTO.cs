using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ADSync.Core.DTO
{
    public class ResultadoSincronismoDTO
    {
        public UsuarioDTO Usuario { get; set; }

        public bool Sucesso { get; set; }

        public string MensagemErro { get; set; }
        
    }

}
