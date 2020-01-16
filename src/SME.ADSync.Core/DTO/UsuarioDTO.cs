using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ADSync.Core.DTO
{
    public class UsuarioDTO
    {
        public Guid Id { get; set; }
        public string PrimeiroNome { get; set; }
        public string Sobrenome { get; set; }
        public string Login { get; set; }
        public string Email { get; set; }
        public int Criptografia { get; set; }
        public string Senha { get; set; }
        public string OU { get; set; }
        public string Descricao { get; set; }
        public short Situacao { get; set; }
        public DateTime DataAlteracao { get; set; }

        public static bool operator ==(UsuarioDTO sideA, UsuarioDTO sideB)
        {
            if (object.ReferenceEquals(sideA, null))
            {
                return object.ReferenceEquals(sideB, null);
            }

            return sideA.Equals(sideB);
        }

        public static bool operator !=(UsuarioDTO sideA, UsuarioDTO sideB)
        {
            if (object.ReferenceEquals(sideA, null))
            {
                return !object.ReferenceEquals(sideB, null);
            }

            return !sideA.Equals(sideB);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var sideA = this;
            var sideB = (UsuarioDTO)obj;

            return
                sideA.Login.ToUpper() == sideB.Login.ToUpper();
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.AppendLine("==============================================================");
            str.AppendLine($"Login:             {Login}");
            str.AppendLine($"Primeiro Nome:     {PrimeiroNome}");
            str.AppendLine($"Sobrenome:         {Sobrenome}");
            str.AppendLine($"OU:                {OU}");
            str.AppendLine("==============================================================");
            return str.ToString();
        }
    }
}
