using System;
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
            sqlQuery.AppendLine("UPDATE INTO Sincronizacao");
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

        ~RepositorioADSync()
        {
            if (connection.State == ConnectionState.Open)
                connection.Close();
            connection.Dispose();
        }
    }
}
