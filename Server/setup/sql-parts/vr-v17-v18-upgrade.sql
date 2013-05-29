--==========================================================================
--
-- VR Estate database upgrade script v17 -> v18
--
-- Script version 1
-- Last Modified on 2013-05-25 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 17 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		ALTER TABLE [dbo].[Users]
			ADD
				[RefererRestriction] [nvarchar](MAX) NULL

		UPDATE [DbSettings] SET [IntValue] = 17, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 18, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
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
