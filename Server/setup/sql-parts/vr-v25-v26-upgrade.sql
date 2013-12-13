--==========================================================================
--
-- VR Estate database upgrade script v25 -> v26
--
-- Script version 1
-- Last Modified on 2013-11-20 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 25 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		ALTER TABLE [dbo].[Sites]
			ADD
				[PoiModelUrl] [nvarchar](256) NULL

		CREATE TABLE [dbo].[Structures]
		(
			[AutoID] [int] IDENTITY(1,1) NOT NULL,
			[Version] [timestamp] NOT NULL,
			[Created] [datetime] NOT NULL,
			[Updated] [datetime] NOT NULL,
			[Deleted] [bit] NOT NULL,
			[DisplayModelUrl] [nvarchar](256) NULL,
			[PosLongitude] [real] NULL,
			[PosLatitude] [real] NULL,
			[PosAltitude] [real] NULL,
			[AltitudeAdjustment] [float] NULL,
			CONSTRAINT [PK_Structures] PRIMARY KEY CLUSTERED 
			(
				[AutoID] ASC
			)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
		) ON [PRIMARY]

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



		DECLARE @sid INT
		DECLARE @poi NVARCHAR(256)

		DECLARE cc CURSOR FOR SELECT [AutoID] FROM [Sites] WHERE [Deleted] = 0
		OPEN cc

		FETCH FROM cc INTO @sid
		WHILE @@FETCH_STATUS = 0 BEGIN

			SET @poi = NULL
			SELECT TOP 1 @poi = [PoiModelUrl] FROM [Buildings]
				WHERE [Deleted] = 0 AND [SiteId] = @sid AND [PoiModelUrl] IS NOT NULL
	
			IF @poi IS NOT NULL BEGIN
				UPDATE [Sites] SET [Updated] = GETUTCDATE(), [PoiModelUrl] = @poi WHERE [AutoID] = @sid
			END

			FETCH FROM cc INTO @sid
		END

		CLOSE cc
		DEALLOCATE cc


		UPDATE [Buildings] SET [Updated] = GETUTCDATE(), [PoiModelUrl] = NULL WHERE [PoiModelUrl] IS NOT NULL



		UPDATE [DbSettings] SET [IntValue] = 21, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 26, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Complete.'
END
GO
