--==========================================================================
--
-- VR Estate database upgrade script v20 -> v21
--
-- Script version 1
-- Last Modified on 2013-06-19 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 20 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		CREATE TABLE [dbo].[MlsInfo]
		(
			[AutoID] [int] IDENTITY(1,1) NOT NULL,
			[Version] [timestamp] NOT NULL,
			[Created] [datetime] NOT NULL,
			[Updated] [datetime] NOT NULL,
			[Deleted] [bit] NOT NULL,
			[MlsNum] [nvarchar](32) NOT NULL,
			[RawInfo] [nvarchar](MAX) NOT NULL,
			CONSTRAINT [PK__aMlsInfo] PRIMARY KEY CLUSTERED 
			(
				[AutoID] ASC
			) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
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
	PRINT 'Moving data...'
	BEGIN TRAN

		DECLARE @id nvarchar(32)
		DECLARE @info nvarchar(MAX)

		DECLARE mi CURSOR FOR SELECT [MlsId], [RawMlsInfo] FROM [dbo].[ViewOrders]
			WHERE [MlsId] IS NOT NULL AND [MlsId] <> '' AND [RawMlsInfo] IS NOT NULL AND [Deleted] = 0
		OPEN mi

		FETCH NEXT FROM mi INTO @id, @info
		WHILE @@FETCH_STATUS = 0 BEGIN

			INSERT INTO [dbo].[MlsInfo] ([Created],[Updated],[Deleted],[MlsNum],[RawInfo])
				VALUES (GETUTCDATE(), GETUTCDATE(), 0, @id, @info)

			FETCH NEXT FROM mi INTO @id, @info
		END

		CLOSE mi
		DEALLOCATE mi

		UPDATE [DbSettings] SET [IntValue] = 2, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Complete.'
END
GO


DECLARE @stage INT

SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @stage = 2
BEGIN
	PRINT 'Finalizing...'
	BEGIN TRAN

		CREATE NONCLUSTERED INDEX [IX_MlsInfo_Num] ON [dbo].[MlsInfo]
		(
			[MlsNum]
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]

		ALTER TABLE [dbo].[ViewOrders] DROP COLUMN [RawMlsInfo]
		

		UPDATE [DbSettings] SET [IntValue] = 21, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 21, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Complete.'
END
GO
