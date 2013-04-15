--==========================================================================
--
-- VR Estate database upgrade script v14 -> v15
--
-- Script version 1
-- Last Modified on 2013-04-15 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 14 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		EXEC sp_rename 'dbo.Sites.BubbleTemplateUrl', 'BubbleWebTemplateUrl', 'COLUMN'
		EXEC sp_rename 'dbo.Buildings.BubbleTemplateUrl', 'BubbleWebTemplateUrl', 'COLUMN'

		ALTER TABLE [dbo].[Sites]
			ADD
				[BubbleKioskTemplateUrl] [nvarchar](256) NULL

		ALTER TABLE [dbo].[Buildings]
			ADD
				[BubbleKioskTemplateUrl] [nvarchar](256) NULL

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
	PRINT 'Data transfers...'
	BEGIN TRAN

		-- ----------------------------------------------------------------------
		-- Fixing data
		-- ----------------------------------------------------------------------

		UPDATE [dbo].[Sites] SET [Updated] = GETUTCDATE(), [BubbleKioskTemplateUrl] = [BubbleWebTemplateUrl]
			WHERE [BubbleWebTemplateUrl] IS NOT NULL

		UPDATE [dbo].[Buildings] SET [Updated] = GETUTCDATE(), [BubbleKioskTemplateUrl] = [BubbleWebTemplateUrl]
			WHERE [BubbleWebTemplateUrl] IS NOT NULL

		UPDATE [DbSettings] SET [IntValue] = 15, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 15, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Data transfers complete.'
END
GO