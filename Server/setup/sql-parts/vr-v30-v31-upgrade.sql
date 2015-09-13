--==========================================================================
--
-- VR Estate database upgrade script v30 -> v31
--
-- Script version 1
-- Last Modified on 2015-08-23 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 30 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		ALTER TABLE [dbo].[Buildings]
			ADD
				[LocalizedName] [nvarchar](256) NOT NULL DEFAULT ''

		ALTER TABLE [dbo].[Sites]
			ADD
				[LocalizedName] [nvarchar](256) NOT NULL DEFAULT ''

		ALTER TABLE [dbo].[Structures]
			ADD
				[LocalizedName] [nvarchar](256) NOT NULL DEFAULT ''

		ALTER TABLE [dbo].[SuiteLevels]
			ADD
				[LocalizedName] [nvarchar](256) NOT NULL DEFAULT ''

		ALTER TABLE [dbo].[Suites]
			ADD
				[LocalizedFloorName] [nvarchar](256) NOT NULL DEFAULT '',
				[LocalizedSuiteName] [nvarchar](256) NOT NULL DEFAULT ''

		ALTER TABLE [dbo].[SuiteTypes]
			ADD
				[LocalizedName] [nvarchar](256) NOT NULL DEFAULT ''


		UPDATE [DbSettings] SET [IntValue] = -1, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = -1, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 1, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

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
	PRINT 'Data fix-ups...'
	BEGIN TRAN


		UPDATE [dbo].[Buildings] SET [LocalizedName] = [Name]
		UPDATE [dbo].[Sites] SET [LocalizedName] = [Name]
		UPDATE [dbo].[Structures] SET [LocalizedName] = [Name]
		UPDATE [dbo].[SuiteLevels] SET [LocalizedName] = [Name]
		UPDATE [dbo].[Suites] SET [LocalizedFloorName] = [FloorName], [LocalizedSuiteName] = [SuiteName]
		UPDATE [dbo].[SuiteTypes] SET [LocalizedName] = [Name]


		UPDATE [DbSettings] SET [IntValue] = 28, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 31, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Complete.'
END
GO
