using System;
using System.Collections.Generic;
using System.Text;

namespace SME.ADSync.Core.Interfaces
{
    public interface IConsultaOU
    {
        string MontarOUUsuario(string login, string oU);
    }
}
