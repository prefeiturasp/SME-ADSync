using Dapper;
using SME.ADSync.Core.DTO;
using SME.ADSync.Core.Enumerados;
using SME.ADSync.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace SME.SyncADxCoreSSO.Repositorios
{
    public class RepositorioCoreSSO : IRepositorioCoreSSO, IConsultaOU
    {
        SqlConnection connection = null;

        public RepositorioCoreSSO(string connectionString)
        {
            connection = new SqlConnection(connectionString);
            connection.Open();
        }
        public bool CriarUsuario(UsuarioDTO user)
        {
            return false;
        }

        public IEnumerable<ResultadoSincronismoDTO> CriarUsuario(IEnumerable<UsuarioDTO> users)
        {
            return Enumerable.Empty<ResultadoSincronismoDTO>();
        }

        public UsuarioDTO ObterUmOuPadrao(string logon)
        {
            return connection.QueryFirstOrDefault<UsuarioDTO>($"{Query} AND Login = '{logon}'");
        }

        public IEnumerable<UsuarioDTO> ObterParaComparacao()
        {            
            return connection.Query<UsuarioDTO>(Query, commandTimeout: 120);
        }

        public IEnumerable<UsuarioDTO> Listar(string[] logons)
        {
            var toFilter = logons.Select(x => $"'{x}'");
            return connection.Query<UsuarioDTO>($"{Query} AND Logon in ({string.Join(",", toFilter)})");
        }

        public string MontarOUUsuario(string login, string OU)
        {
            if (string.IsNullOrWhiteSpace(OU))
                return ObterOUPelaUnidadeAdministrativa(connection.Query<UnidadeAdministrativaDTO>(QueryConsultaOU.Replace("#usu_login", login)).FirstOrDefault());
            else
                return
                    $"OU=Users,OU={OU},OU=DRE";
        }

        private string ObterOUPelaUnidadeAdministrativa(UnidadeAdministrativaDTO unidadeAdministrativaDTO)
        {
            var ou = ObterOuPorNomeResumido(unidadeAdministrativaDTO?.NomeResumido);
            return !string.IsNullOrWhiteSpace(ou) ? $"OU=Users,OU={ou},OU=DRE" : null;
        }

        private string ObterOuPorNomeResumido(string nomeResumido)
        {
            if (string.IsNullOrWhiteSpace(nomeResumido))
                return null;

            switch (nomeResumido)
            {
                case "BUTANTA":
                    return "Butanta";

                case "CAMPO LIMPO":
                    return "Campo-Limpo";

                case "CAPELA DO SOCORRO":
                    return "Capela-Socorro";

                case "FREGUESIA/BRASILANDIA":
                    return "Freguesia";

                case "GUAIANASES":
                    return "Guaianases";

                case "IPIRANGA":
                    return "Ipiranga";

                case "ITAQUERA":
                    return "Itaquera";

                case "JACANA/TREMEMBE":
                    return "Jacana";

                case "PENHA":
                    return "Penha";

                case "PIRITUBA/JARAGUA":
                    return "Pirituba";

                case "SANTO AMARO":
                    return "Santo-Amaro";

                case "SAO MATEUS":
                    return "Sao-Mateus";

                case "SAO MIGUEL":
                    return "Sao-Miguel";

                default:
                    return null;
            }
        }

        public bool ResetarSenhaParaPadrao(UsuarioDTO user, string conextoLog = "")
        {
            var encrypt = new MSTech.Security.Cryptography.SymmetricAlgorithm(MSTech.Security.Cryptography.SymmetricAlgorithm.Tipo.TripleDES);
            var sqlQuery = new StringBuilder();

            sqlQuery.AppendLine("UPDATE SYS_Usuario");
            sqlQuery.AppendLine($"SET usu_criptografia = {(int)TipoCriptografia.TripleDES},");
            sqlQuery.AppendLine($"    usu_senha = '{ encrypt.Encrypt(ObterSenhaPadrao(user.Login))}',");
            sqlQuery.AppendLine($"    usu_dataAlteracao = '{ DateTime.Now }'");
            sqlQuery.AppendLine($"WHERE usu_id = '{user.Id}'");

            return connection.Execute(sqlQuery.ToString()) > 0;
        }

        public string ObterSenhaPadrao(string login)
        {
            return $"Sgp{login.PadLeft(4, '0').Substring(login.Length - 4, 4)}";
        }

        const string Query =
        @"DECLARE @Lista TABLE (usu_id UNIQUEIDENTIFIER,
					            pes_nome VARCHAR(200),
					            usu_login VARCHAR(500),
					            usu_dataCriacao DATETIME,
                                usu_dataAlteracao DATETIME,
					            usu_criptografia TINYINT,
					            usu_senha VARCHAR(256),
                                usu_situacao TINYINT,
					            professor BIT,
					            gestor BIT)
         INSERT INTO @Lista
         SELECT u.usu_id,
	            p.pes_nome,
	            u.usu_login,
	            u.usu_dataCriacao,
                u.usu_dataAlteracao,
	            u.usu_criptografia,
	            u.usu_senha,
                u.usu_situacao,
	            'True' professor,
	            'False' gestor
	         FROM SYS_Usuario u (NOLOCK)
		         INNER JOIN PES_Pessoa p (NOLOCK)
		 	        ON u.pes_id = p.pes_id
		         INNER JOIN SYS_UsuarioGrupo ug (NOLOCK)
		 	        ON u.usu_id = ug.usu_id
		         INNER JOIN SYS_Grupo g (NOLOCK)
		 	        ON ug.gru_id = g.gru_id
         WHERE u.usu_dataCriacao BETWEEN '2016-07-01' AND GETDATE() AND
	           g.sis_id = 102 AND
	           g.gru_nome LIKE '%docente%'
           
         UNION
           
         SELECT u.usu_id,
	            p.pes_nome,
	            u.usu_login,
	            u.usu_dataCriacao,
                u.usu_dataAlteracao,
	            u.usu_criptografia,
	            u.usu_senha,
                u.usu_situacao,
	            'False' professor,
	            'True' gestor
	         FROM SYS_Usuario u (NOLOCK)
		         INNER JOIN PES_Pessoa p (NOLOCK)
		 	        ON u.pes_id = p.pes_id
		         INNER JOIN SYS_UsuarioGrupo ug (NOLOCK)
		 	        ON u.usu_id = ug.usu_id
		         INNER JOIN SYS_Grupo g (NOLOCK)
		 	        ON ug.gru_id = g.gru_id
         WHERE g.sis_id = 102 AND
	           g.gru_id in ('28880585-89C5-E311-B1FE-782BCB3D2D76', --Assistente de Diretor na UE
				            '85A1DF73-9DC5-E311-B1FE-782BCB3D2D76', --SGP - AD
				            '3EC44693-92AF-E311-B1FE-782BCB3D2D76', --SGP - Coordenador Pedagógico
				            '077B5042-A9B6-E311-B1FE-782BCB3D2D76', --SGP - Diretor Escolar
				            'EA7EC579-B006-479E-BCA6-4A7BEBB5412E', --SGP - Diretor Inf. Terceirizado
				            '8D32E519-C55E-E411-819D-782BCB3D218E', --SGP - Diretor Regional
				            '3EE28335-163A-4D2B-9A4A-9B9F09EE01A6', --SGP - Sec Escolar Inf Terceirizado
				            'CACC8667-EB64-494D-B31A-88FD1E2C3904', --SGP - Sec. Escolar Infantil
				            '8E49D1FE-326B-E411-819D-782BCB3D218E') --SGP - Secretário Escolar
           
         SELECT DISTINCT usu_id [Id],
                         SUBSTRING(LTRIM(pes_nome), 1, CHARINDEX(' ', pes_nome)) [PrimeiroNome], 
		   		         SUBSTRING(pes_nome, CHARINDEX(' ', pes_nome), LEN(pes_nome) - CHARINDEX(' ', pes_nome) + 1) [Sobrenome], 
		   		         usu_login [Login], 
		   		         usu_datacriacao [DataCriacao], 
                         usu_dataAlteracao [DataAlteracao],
		   		         usu_criptografia [Criptografia],
		   		         usu_senha [Senha],
                         usu_situacao [Situacao],
		   	             ISNULL((SELECT 'S'
		   			             FROM @Lista
		   				         WHERE usu_id = l.usu_id AND
		   				  	           professor = 'True'), 'N') Eh_Professor,
		   		         ISNULL((SELECT 'S'
		   				         FROM @Lista
		   				         WHERE usu_id = l.usu_id AND
		   				  	           gestor = 'True'), 'N') Eh_Gestor,
		   		         DATEADD(MONTH, -40, GETDATE()) HojeMenos40Meses
	         FROM @Lista l
         ORDER BY l.usu_login";

        const string QueryConsultaOU = @"
            DECLARE @usu_login VARCHAR(20); 
            DECLARE @uad_pai UNIQUEIDENTIFIER; 
            DECLARE @uad_corrente UNIQUEIDENTIFIER; 
            BEGIN 
                SET @usu_login = '#usu_login'; 
                SELECT @uad_pai = uad_idsuperior, @uad_corrente = uad_id
                FROM   (SELECT /*u.usu_id, 
                               gua.gru_id, 
                               gua.ugu_id, 
                               u.usu_login, 
                               uad_nome, */
				               gua.uad_id,
                               ua.uad_idsuperior, 
                               --g.usg_situacao, 
                               Rank() 
                                 OVER ( 
                                   partition BY u.usu_login 
                                   ORDER BY u.usu_id, gua.gru_id, gua.ugu_id) AS RankID 
                        FROM   dbo.sys_usuario u (NOLOCK)
                               INNER JOIN dbo.sys_usuariogrupoua gua (NOLOCK)
                                       ON gua.usu_id = u.usu_id 
                               INNER JOIN dbo.sys_unidadeadministrativa ua (NOLOCK)
                                       ON ua.uad_id = gua.uad_id 
							              AND ua.uad_situacao = 1
                               INNER JOIN sys_usuariogrupo g (NOLOCK)
                                       ON g.gru_id = gua.gru_id 
                                          AND g.usu_id = gua.usu_id 
                                          AND g.usg_situacao = 1 
                        WHERE  u.usu_login = @usu_login) a 
                WHERE  rankid = 1;
	            WHILE NOT( @uad_pai IS NULL ) 
	            BEGIN 
		            --PRINT @uad_pai; 
		            --PRINT @uad_corrente; 
		            SELECT @uad_pai = uad_idsuperior, 
			               @uad_corrente = uad_id 
		            FROM   dbo.sys_unidadeadministrativa ua (NOLOCK)
		            WHERE  ua.uad_id = @uad_pai; 
	            END;  
	            --print @uad_pai;
	            --print @uad_corrente;
	            SELECT ua.uad_id Id, 
		               ua.uad_nome Nome,
		               replace(ua.uad_nome, 'DIRETORIA REGIONAL DE EDUCACAO ', '') NomeResumido,
		               ua.uad_codigo Codigo, 
		               ua.uad_codigoinep CodigoInep, 
		               ua.uad_sigla Sigla, 
		               ua.uad_situacao Situacao 
	            FROM   dbo.sys_unidadeadministrativa ua (NOLOCK)
	            WHERE  ua.uad_id = @uad_corrente; 
            END;";
    }
}
