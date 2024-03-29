USE [master]
GO

CREATE DATABASE [ADSync]
GO

USE [ADSync]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sincronizacao](
	[UsuarioIdCoreSSO] [uniqueidentifier] NOT NULL,
	[DataUltimaSincronizacao] [datetime] NOT NULL,
	[Ativo] [bit] NOT NULL,
 CONSTRAINT [PK_Sincronizacao] PRIMARY KEY CLUSTERED 
(
	[UsuarioIdCoreSSO] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Sincronizacao] ADD  CONSTRAINT [DF_Sincronizacao_Ativo]  DEFAULT ((1)) FOR [Ativo]
GO

CREATE TABLE [dbo].[ResultadoSincronismo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[DataHora] [datetime] NOT NULL,
	[Sucesso] [bit] NULL,
	[ResultadoImportacao] [tinyint] NULL,
	[MensagemErro] [varchar](max) NULL,
 CONSTRAINT [PK_ResultadoSincronismo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[ResultadoSincronismo] ADD  CONSTRAINT [DF_ResultadoSincronismo_DataHora]  DEFAULT (getdate()) FOR [DataHora]
GO

CREATE TABLE [dbo].[UsuarioResultadoSincronismo](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[ResultadoSincronismoId] [bigint] NOT NULL,
	[UsuarioId] [uniqueidentifier] NOT NULL,
	[PrimeiroNome] [varchar](250) NOT NULL,
	[Sobrenome] [varchar](1000) NULL,
	[Login] [varchar](50) NOT NULL,
	[Email] [varchar](250) NULL,
	[Criptografia] [tinyint] NOT NULL,	
	[OU] [varchar](1000) NULL,
	[Descricao] [varchar](2000) NULL,
	[Situacao] [tinyint] NOT NULL,
	[DataCriacao] [datetime] NOT NULL,
	[DataAlteracao] [datetime] NULL,
	[Erro] [varchar](1500) NULL,
	[Professor] [bit] NOT NULL,
	[Gestor] [bit] NOT NULL,
	[HojeMenos40Meses] [datetime] NULL,
 CONSTRAINT [PK_UsuarioResultadoSincronismo] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, FILLFACTOR = 90) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[UsuarioResultadoSincronismo]  WITH CHECK ADD  CONSTRAINT [FK_UsuarioResultadoSincronismo_ResultadoSincronismo] FOREIGN KEY([ResultadoSincronismoId])
REFERENCES [dbo].[ResultadoSincronismo] ([Id])
ON DELETE CASCADE
GO

ALTER TABLE [dbo].[UsuarioResultadoSincronismo] CHECK CONSTRAINT [FK_UsuarioResultadoSincronismo_ResultadoSincronismo]
GO

USE [master]
GO
ALTER DATABASE [ADSync] SET  READ_WRITE 
GO
