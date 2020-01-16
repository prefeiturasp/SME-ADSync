using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ADSync.Core.DTO
{
    public class UnidadeAdministrativaDTO
    {
        public object Id { get; set; }
        public string Nome { get; set; }
        public string NomeResumido { get; set; }
        public string Codigo { get; set; }
        public string CodigoINEP { get; set; }
        public string Sigla { get; set; }
        public int Situacao { get; set; }
    }
}
