--==========================================================================
--
-- VR Estate database upgrade script v21 -> v22
--
-- Script version 1
-- Last Modified on 2013-06-22 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 21 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		EXEC sp_rename 'dbo.SuiteTypes.Model', 'WireframeModel', 'COLUMN'

		ALTER TABLE [dbo].[Sites]
			ALTER COLUMN [DisplayModelUrl] [nvarchar](256) NULL

		ALTER TABLE [dbo].[Buildings]
			ALTER COLUMN [DisplayModelUrl] [nvarchar](256) NULL

		ALTER TABLE [dbo].[Buildings]
			ADD
				[CenterLongitude] [real] NULL,
				[CenterLatitude] [real] NULL,
				[CenterAltitude] [real] NULL,
				[MaxSuiteAlt] [real] NULL

		UPDATE [dbo].[Suites] SET [Updated] = GETUTCDATE(), [CeilingHeight] = [CeilingHeight] + ',0' WHERE [CeilingHeight] IS NOT NULL

		UPDATE [DbSettings] SET [IntValue] = 21, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 22, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
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
