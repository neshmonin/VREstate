--==========================================================================
--
-- VR Estate database upgrade script v22 -> v23
--
-- Script version 1
-- Last Modified on 2013-07-21 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 22 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		ALTER TABLE [dbo].[Suites]
			ALTER COLUMN [FloorName] [nvarchar](32) NULL

		ALTER TABLE [dbo].[Suites]
			ALTER COLUMN [SuiteName] [nvarchar](32) NULL


		UPDATE [DbSettings] SET [IntValue] = 21, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 23, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
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
