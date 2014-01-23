--==========================================================================
--
-- VR Estate database upgrade script v29 -> v30
--
-- Script version 1
-- Last Modified on 2014-01-19 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 29 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		CREATE TABLE [dbo].[Invoice]
		(
			[AutoID] [int] IDENTITY(1,1) NOT NULL,
			[Created] [datetime] NOT NULL,
			[TargetObjectType] [int] NOT NULL,
			[TargetObjectId] [int] NOT NULL,
			[Contents] [nvarchar](max) NOT NULL,
			CONSTRAINT [PK_Invoice] PRIMARY KEY CLUSTERED 
			(
				[AutoID] ASC
			)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]


		UPDATE [DbSettings] SET [IntValue] = 28, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 30, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'DB schema upgrade complete.'
END
ELSE IF @ver <> -1
BEGIN
	PRINT 'ERROR: DB Version is not correct.'
	RETURN
END
GO
