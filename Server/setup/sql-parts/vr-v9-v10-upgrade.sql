--==========================================================================
--
-- VR Estate database upgrade script v9 -> v10
--
-- Adds
--
-- Script version 1
-- Last Modified on 2013-01-09 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 9 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		EXEC sp_rename 'dbo.SuiteTypes.BathroomCnt', 'SBathroomCnt', 'COLUMN'

		ALTER TABLE [dbo].[SuiteTypes]
			ADD
				[NSBathroomCnt] [int] NULL,
				[OtherRoomCnt] [int] NULL

		ALTER TABLE [dbo].[ViewOrders]
			ADD
				[MlsUrl] [nvarchar](max) NULL,
				[Note] [nvarchar](max) NULL

		UPDATE [DbSettings] SET [IntValue] = -1, [TimeValue] = GETDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = -1, [TimeValue] = GETDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 1, [TimeValue] = GETDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'DB schema upgrade complete.'
END
ELSE IF @ver <> -1
BEGIN
	PRINT 'ERROR: DB Version is not correct.'
	RETURN
END
GO



DECLARE @stage INT

SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @stage = 1
BEGIN
	PRINT 'Data transfers...'
	BEGIN TRAN

		-- ----------------------------------------------------------------------
		-- Fixing data
		-- ----------------------------------------------------------------------

		UPDATE [dbo].[SuiteTypes] SET [NSBathroomCnt] = 0
		UPDATE [dbo].[SuiteTypes] SET [Updated] = GETUTCDATE(), [OtherRoomCnt] = 1 WHERE [BedroomCnt] = 0

		UPDATE [DbSettings] SET [IntValue] = 10, [TimeValue] = GETDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 10, [TimeValue] = GETDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Data transfers complete.'
END
GO