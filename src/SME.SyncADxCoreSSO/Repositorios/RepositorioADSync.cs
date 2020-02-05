using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Dapper;
using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Interfaces;

namespace SME.SyncADxCoreSSO.Repositorios
{
    public class RepositorioADSync : IRepositorioADSync
    {
        private readonly SqlConnection connection;

        public RepositorioADSync(string connectionString)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
                throw new ArgumentNullException(nameof(connectionString));

            connection = new SqlConnection(connectionString);
            connection.Open();
        }

        public void AtualizarSincronizacao(SincronizacaoDTO sincronizacao)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("UPDATE Sincronizacao");
            sqlQuery.AppendLine("SET DataUltimaSincronizacao = @DataUltimaSincronizacao,");
            sqlQuery.AppendLine("    Ativo = @Ativo");
            sqlQuery.AppendLine("WHERE UsuarioIdCoreSSO = @UsuarioIdCoreSSO");

            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioIdCoreSSO", sincronizacao.UsuarioIdCoreSSO);
            parameters.Add("@DataUltimaSincronizacao", sincronizacao.DataUltimaSincronizacao);
            parameters.Add("@Ativo", sincronizacao.Ativo);

            connection.Execute(sqlQuery.ToString(), parameters);
        }

        public void IncluirSincronizacao(SincronizacaoDTO sincronizacao)
        {
            var sqlQuery = new StringBuilder();
            sqlQuery.AppendLine("INSERT INTO Sincronizacao (UsuarioIdCoreSSO, DataUltimaSincronizacao, Ativo)");
            sqlQuery.AppendLine("VALUES (@UsuarioIdCoreSSO, @DataUltimaSincronizacao, @Ativo)");

            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioIdCoreSSO", sincronizacao.UsuarioIdCoreSSO);
            parameters.Add("@DataUltimaSincronizacao", sincronizacao.DataUltimaSincronizacao);
            parameters.Add("@Ativo", sincronizacao.Ativo);

            connection.Execute(sqlQuery.ToString(), parameters);
        }

        public SincronizacaoDTO ObterSincronizacao(Guid usuarioIdCoreSSO)
        {
            var parameters = new DynamicParameters();
            parameters.Add("@UsuarioIdCoreSSO", usuarioIdCoreSSO);
            return connection.Query<SincronizacaoDTO>(
                @"SELECT UsuarioIdCoreSSO,              
                         DataUltimaSincronizacao, 
                         Ativo 
                    FROM Sincronizacao 
                  WHERE UsuarioIdCoreSSO = @UsuarioIdCoreSSO", parameters)
                .SingleOrDefault();
        }

        public void IncluirResultadoSincronizacao<T>(T resultado) where T : ResultadoBaseDTO
        {
            var sqlQuery = new StringBuilder();

            sqlQuery.AppendLine("INSERT INTO ResultadoSincronismo (Sucesso, ResultadoImportacao, MensagemErro)");
            sqlQuery.AppendLine("VALUES (@Sucesso, @ResultadoImportacao, @MensagemErro)");
            sqlQuery.AppendLine("DECLARE @ResultadoSincId BIGINT = @@Identity");
            sqlQuery.AppendLine("INSERT INTO UsuarioResultadoSincronismo (ResultadoSincronismoId,");
            sqlQuery.AppendLine("                                         UsuarioId,");
            sqlQuery.AppendLine("                                         PrimeiroNome,");
            sqlQuery.AppendLine("                                         Sobrenome,");
            sqlQuery.AppendLine("                                         Login,");
            sqlQuery.AppendLine("                                         Email,");
            sqlQuery.AppendLine("                                         Criptografia,");
            sqlQuery.AppendLine("                                         OU,");
            sqlQuery.AppendLine("                                         Descricao,");
            sqlQuery.AppendLine("                                         Situacao,");
            sqlQuery.AppendLine("                                         DataCriacao,");
            sqlQuery.AppendLine("                                         DataAlteracao,");
            sqlQuery.AppendLine("                                         Erro,");
            sqlQuery.AppendLine("                                         Professor,");
            sqlQuery.AppendLine("                                         Gestor,");
            sqlQuery.AppendLine("                                         HojeMenos40Meses)");
            sqlQuery.AppendLine("VALUES (@ResultadoSincId, @UsuarioId, @PrimeiroNome, @Sobrenome, @Login,");
            sqlQuery.AppendLine("        @Email, @Criptografia, @OU, @Descricao, @Situacao, @DataCriacao,");
            sqlQuery.AppendLine("        @DataAlteracao, @Erro, @Professor, @Gestor, @HojeMenos40Meses)");

            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@Sucesso", resultado.GetType().Equals(typeof(ResultadoSincronismoDTO)) ? (resultado as ResultadoSincronismoDTO).Sucesso : (bool?)null);
                parameters.Add("@ResultadoImportacao", resultado.GetType().Equals(typeof(ResultadoImportacaoDTO)) ? (short)(resultado as ResultadoImportacaoDTO).Resultado : (short?)null);
                parameters.Add("@MensagemErro", resultado.MensagemErro);
                parameters.Add("@UsuarioId", resultado.Usuario.Id);
                parameters.Add("@PrimeiroNome", resultado.Usuario.PrimeiroNome);
                parameters.Add("@Sobrenome", resultado.Usuario.Sobrenome);
                parameters.Add("@Login", resultado.Usuario.Login);
                parameters.Add("@Email", resultado.Usuario.Email);
                parameters.Add("@Criptografia", resultado.Usuario.Criptografia);
                parameters.Add("@OU", resultado.Usuario.OU);
                parameters.Add("@Descricao", resultado.Usuario.Descricao);
                parameters.Add("@Situacao", resultado.Usuario.Situacao);
                parameters.Add("@DataCriacao", resultado.Usuario.DataCriacao);
                parameters.Add("@DataAlteracao", resultado.Usuario.DataAlteracao);
                parameters.Add("@Erro", resultado.Usuario.Erro);
                parameters.Add("@Professor", resultado.Usuario.Eh_Professor.Equals('S'));
                parameters.Add("@Gestor", resultado.Usuario.Eh_Gestor.Equals('S'));
                parameters.Add("@HojeMenos40Meses", resultado.Usuario.HojeMenos40Meses);
                connection.Execute(sqlQuery.ToString(), parameters);
            }
            catch (Exception ex)
            {
                System.Diagnostics.EventLog.WriteEntry("ADSync", ex.ToString(), System.Diagnostics.EventLogEntryType.Error);
            }
        }

        ~RepositorioADSync()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
            connection.Dispose();
        }
    }
}
