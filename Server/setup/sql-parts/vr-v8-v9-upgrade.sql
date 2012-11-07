--==========================================================================
--
-- VR Estate database upgrade script v8 -> v9
--
-- Adds
--
-- Script version 1
-- Last Modified on 2012-11-07 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 8 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		EXEC sp_rename 'dbo.Listings', 'ViewOrders', 'OBJECT'

		UPDATE [DbSettings] SET [IntValue] = 9, [TimeValue] = GETDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 9, [TimeValue] = GETDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'DB upgrade complete.'
END
GO
