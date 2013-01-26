--==========================================================================
--
-- VR Estate database upgrade script v11 -> v12
--
-- Script version 1
-- Last Modified on 2013-01-25 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 11 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		EXEC sp_rename 'dbo.ViewOrders.ProductUrl', 'VTourUrl', 'COLUMN'
		EXEC sp_rename 'dbo.ViewOrders.MlsUrl', 'InfoUrl', 'COLUMN'

		UPDATE [DbSettings] SET [IntValue] = 12, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 12, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
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
