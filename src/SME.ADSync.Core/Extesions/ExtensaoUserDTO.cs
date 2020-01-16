using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Text;

namespace SME.ADSync.Core.Extensoes
{
    public static class ExtensaoUserDTO
    {
        public static UsuarioDTO ParaUsurarioDTO(this UserPrincipal @this)
        {
            if (@this != null)
            {
                return @this != null ?
                new UsuarioDTO()
                {

                    Login = @this.SamAccountName,
                    PrimeiroNome = @this.GivenName,
                    Sobrenome = @this.Surname,
                    Email = @this.EmailAddress,
                    OU = @this.DistinguishedName,
                    Descricao = @this.Description
                } : null;
            }
            else return null;
        }
    }
}
