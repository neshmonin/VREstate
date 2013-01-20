--==========================================================================
--
-- VR Estate database upgrade script v10 -> v11
--
-- Script version 1
-- Last Modified on 2013-01-19 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 10 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		EXEC sp_rename 'dbo.ViewOrders.Product', 'Options', 'COLUMN'

		ALTER TABLE [dbo].[ViewOrders]
			ADD
				[Product] [int] NULL

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

		UPDATE [dbo].[ViewOrders] SET [Product] = 0

		UPDATE [dbo].[Suites] SET [Updated] = GETUTCDATE(), [Status] = 2
			WHERE [Status] = 3 AND [Deleted] = 0 AND [AutoID] IN 
			(
				SELECT [TargetObjectId] FROM [dbo].[ViewOrders]
					WHERE [TargetObjectType] = 0 AND [Deleted] = 0 AND [Enabled] = 1
						AND [ExpiresOn] > GETUTCDATE()
			)

		UPDATE [DbSettings] SET [IntValue] = 11, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 11, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Data transfers complete.'
END
GO