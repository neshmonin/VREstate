--==========================================================================
--
-- VR Estate database upgrade script v16 -> v17
--
-- Script version 2
-- Last Modified on 2013-09-05 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 16 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		ALTER TABLE [dbo].[Suites]
			ADD
				[CurrentPrice] [varchar](64) NULL

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


		DECLARE @sid INT, @oid INT, @sot INT
		DECLARE @p FLOAT
		
		SELECT @sot = [AutoID] FROM [OptionTypes] WHERE [Description] = 'Suite'
		
		DECLARE sc CURSOR FOR SELECT s.[AutoID], o.[AutoID]
			FROM [Suites] s
				INNER JOIN [OptionSuiteMM] osmm ON osmm.[SuiteID] = s.[AutoID] 
				INNER JOIN [Options] o ON o.[AutoID] = osmm.[OptionID] AND o.[TypeID] = @sot
			WHERE s.[Deleted] = 0
		OPEN sc		
		
		FETCH NEXT FROM sc INTO @sid, @oid
		WHILE @@FETCH_STATUS = 0 BEGIN

			SET @p = NULL
		
			SELECT TOP 1 @p = p.[PricePerUnitForBuyer] 
				FROM [Prices] p WHERE p.[OptionID] = @oid
				ORDER BY p.[StartingDate] DESC
				
			IF (@p IS NOT NULL AND @p > 0) BEGIN
				UPDATE [Suites] SET [Updated] = GETUTCDATE(), [CurrentPrice] = 'CAD' + LTRIM(RTRIM(STR(@p)))
					WHERE [AutoID] = @sid
				PRINT CAST(@sid AS varchar(16)) + ' <- ' + LTRIM(RTRIM(STR(@p)))
			END
		
			FETCH NEXT FROM sc INTO @sid, @oid
		END
		
		CLOSE sc
		DEALLOCATE sc

		UPDATE [DbSettings] SET [IntValue] = 17, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 17, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Data transfers complete.'
END
GO