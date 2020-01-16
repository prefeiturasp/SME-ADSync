using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Extensoes;
using SME.ADSync.Core.Extesions;
using SME.ADSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;

namespace SME.ADSync.Core
{
    public abstract class RepositorioADBase : IRepositorioAD
    {
        protected string ContextoLog = Guid.NewGuid().ToString();

        protected PrincipalContext contextoPrincipal;

        public RepositorioADBase(string dominio, string diretorio, string usuario, string senha)
        {
            contextoPrincipal = ObterContexto(dominio, diretorio, usuario, senha);
        }

        public abstract string ObterSenhaPadrao(string login);

        private PrincipalContext ObterContexto(string dominio, string diretorio, string usuario, string senha)
        {
            PrincipalContext contextoPrincipal = null;
            try
            {
                contextoPrincipal = new PrincipalContext(ContextType.Domain, dominio, diretorio, usuario, senha);

                if (!contextoPrincipal.ValidateCredentials(usuario, senha))
                    throw new ApplicationException("Falha ao autenticar");
            }
            catch (Exception e)
            {
                throw new Exception("Falha ao criar o contexto principal " + e);
            }

            return contextoPrincipal;
        }

        public bool CriarUsuario(UsuarioDTO user)
        {
            var usr = ObterUmOuPadrao(user.Login);
            if (usr != null)
            {
                user.Descricao = usr.Descricao;
                user.OU = usr.OU;
                throw new ApplicationException(user.Login + " já existe no AD");
            }

            switch ((TipoCriptografia)user.Criptografia)
            {
                case TipoCriptografia.TripleDES:
                    user.Senha = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES).Decrypt(user.Senha);
                    break;
                case TipoCriptografia.MD5:
                case TipoCriptografia.SHA512:
                    user.Senha = null;
                    break;
            }

            UserPrincipal usuario = new UserPrincipal(contextoPrincipal, user.Login, user.Senha ?? ObterSenhaPadrao(user.Login), true);

            if (user.Sobrenome != null && user.Sobrenome.Length > 0)
                usuario.Surname = user.Sobrenome;

            if (user.PrimeiroNome != null && user.PrimeiroNome.Length > 0)
                usuario.GivenName = user.PrimeiroNome;

            usuario.Name = $"{user.PrimeiroNome.Trim()} {user.Sobrenome.Trim()}";

            if (user.Email != null && user.Email.Length > 0)
                usuario.EmailAddress = user.Email;

            usuario.Description = $"Incluido pelo batch SMEADSync - {usuario.Name} - {user.Descricao}";

            usuario.Enabled = true;
            usuario.ExpirePasswordNow();

            try
            {
                usuario.Save();
                user.Descricao = usuario.Description;
                user.OU = usuario.DistinguishedName ?? contextoPrincipal.Container;
            }
            catch (Exception e)
            {
                var seraQueIncluiu = ObterUmOuPadrao(user.Login);
                if (seraQueIncluiu == null || seraQueIncluiu.PrimeiroNome != user.PrimeiroNome)
                    throw new Exception("Erro ao salvar usuário: " + e);

                user.Descricao = seraQueIncluiu.Descricao;
                user.OU = seraQueIncluiu.OU;
            }

            return true;
        }

        public IEnumerable<ResultadoSincronismoDTO> CriarUsuario(IEnumerable<UsuarioDTO> usuarios)
        {
            List<ResultadoSincronismoDTO> resultado = new List<ResultadoSincronismoDTO>();

            foreach (var item in usuarios)
                try { resultado.Add(new ResultadoSincronismoDTO() { Usuario = item, Sucesso = CriarUsuario(item) }); }
                catch (Exception ex)
                { resultado.Add(new ResultadoSincronismoDTO() { Usuario = item, Sucesso = false, MensagemErro = ex.ToString() }); }

            return resultado;
        }

        public IEnumerable<UsuarioDTO> Listar(string[] logons)
        {
            List<UsuarioDTO> result = new List<UsuarioDTO>();

            Console.WriteLine($"Processo lento iniciado: {logons.Length} objetos para processar");

            for (int i = 0; i < logons.Length; i++)
            {
                if (i % 100 == 0)
                    Console.WriteLine($"{i} itens processados");

                var user = ObterUmOuPadrao(logons[i]);
                if (user != null)
                    result.Add(user);

                Thread.Sleep(10);
            }

            return result;
        }

        public UsuarioDTO ObterUmOuPadrao(string logon)
        {
            return UserPrincipal.FindByIdentity(contextoPrincipal, logon).ParaUsurarioDTO();
        }

        public abstract IEnumerable<UsuarioDTO> ObterParaComparacao();

        public bool ResetarSenhaParaPadrao(UsuarioDTO user, string conextoLog = "")
        {
            var usr = ObterUmOuPadrao(user.Login);
            if (usr == null)
            {
                throw new ApplicationException(user.Login + " não existe no AD");
            }
            else
            {
                user.Descricao = usr.Descricao;
                user.OU = usr.OU;
            }

            UserPrincipal usuario = UserPrincipal.FindByIdentity(contextoPrincipal, user.Login);

            user.Senha = ObterSenhaPadrao(user.Login);

            usuario.SetPassword(user.Senha);

            try
            {
                usuario.Save();

                if (usuario.IsAccountLockedOut())
                    DesbloquearUsuario(usuario);

                try
                {
                    usuario.PasswordNeverExpires = true;
                    usuario.Save();
                }
                catch(Exception ex)
                {
                    if (!string.IsNullOrWhiteSpace(conextoLog))
                        Log.GravarLinha(conextoLog, new { Usuario = usuario.SamAccountName, Retorno = "Falha", Mesagem = ex.ToString() }, "PasswordExpiration_");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Erro ao salvar usuário: " + ex);
            }

            return true;

        }

        private void DesbloquearUsuario(UserPrincipal usuario)
        {
            try
            {
                string ldapPath = "LDAP://" + usuario.DistinguishedName;

                using (DirectoryEntry de = new DirectoryEntry(ldapPath))
                {
                    // check to see if we have "userAccountControl" in the **properties** of de
                    if (de.Properties.Contains("userAccountControl"))
                    {
                        int m_Val = (int)de.Properties["userAccountControl"][0];
                        de.Properties["userAccountControl"].Value = m_Val | 0x0001;

                        de.CommitChanges();
                    }
                }

                Log.GravarLinha(ContextoLog, new { Usuario = usuario.SamAccountName, Retorno = "Sucesso", Mesagem = "" }, "Desbloqueio_");

            }
            catch (Exception ex)
            {
                Log.GravarLinha(ContextoLog, new { Usuario = usuario.SamAccountName, Retorno = "Falha", Mensagem = ex.ToString() }, "Desbloqueio_");
            }
        }
    }
}
