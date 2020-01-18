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
USE [master]
GO
ALTER DATABASE [ADSync] SET  READ_WRITE 
GO
