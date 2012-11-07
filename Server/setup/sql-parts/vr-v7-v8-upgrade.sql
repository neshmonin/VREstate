--==========================================================================
--
-- VR Estate database upgrade script v7 -> v8
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

IF @ver = 7
BEGIN

	PRINT 'Preliminary schema upgrade...'

	BEGIN TRAN

		-- ----------------------------------------------------------------------
		-- Pre-modifying existing tables
		-- ----------------------------------------------------------------------

		ALTER TABLE [dbo].[Listings]
			ADD
				[Enabled] [bit] NOT NULL DEFAULT 1
				
		ALTER TABLE [dbo].[Listings]
			DROP COLUMN
				[PaymentRefId]

		ALTER TABLE [dbo].[Users]
			ADD
				[CreditUnits] [money] NOT NULL DEFAULT 0

		-- ----------------------------------------------------------------------
		-- Creating new tables
		-- ----------------------------------------------------------------------

		CREATE TABLE [dbo].[FT] (
			[AutoID] [int] NOT NULL IDENTITY(1,1),
			[Created] [datetime] NOT NULL,
			[SystemRefId] [varchar](256) NULL,
			[InitiatorId] [int] NOT NULL,
			[Account] [tinyint] NOT NULL,
			[AccountId] [int] NOT NULL,
			[Operation] [tinyint] NOT NULL,
			[CuAmount] [money] NOT NULL,
			[Subject] [tinyint] NOT NULL,
			[Target] [tinyint] NOT NULL,
			[TargetId] [int] NULL,
			[ExtraTargetInfo] [nvarchar](256) NULL,
			[PaymentSystem] [tinyint] NOT NULL,
			[PaymentrefId] [varchar](256) NULL,
			CONSTRAINT [PK_FT] PRIMARY KEY CLUSTERED ( [AutoID] ASC )
				WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
				ON [PRIMARY]
			) ON [PRIMARY]

		UPDATE [DbSettings] SET [IntValue] = -1 WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = -1 WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 1 WHERE [Id] = 4--([Id], [IntValue], [TimeValue]) VALUES (4, 1, GETDATE())

	COMMIT TRAN
	
	PRINT 'DB Shema upgrade partially complete.'
	
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

		DECLARE @sid int
		DECLARE @num nvarchar(256)
		DECLARE @mnum nvarchar(256)

		DECLARE sc CURSOR FOR SELECT [AutoID] AS [sid], [SuiteName] AS [num] FROM [dbo].[Suites] WHERE [Deleted] = 0
		OPEN sc

		FETCH NEXT FROM sc INTO @sid, @num
		WHILE @@FETCH_STATUS = 0 BEGIN

			SET @mnum = LTRIM(RTRIM(@num))
			WHILE ((LEN(@mnum) > 0) AND (LEFT(@mnum, 1) = '0')) BEGIN SET @mnum = RIGHT(@mnum, LEN(@mnum) - 1) END

			IF @mnum <> @num BEGIN
				UPDATE [dbo].[Suites] SET [SuiteName] = @mnum WHERE [AutoID] = @sid
				PRINT @num + '->' + @mnum
			END

			FETCH NEXT FROM sc INTO @sid, @num
		END

		CLOSE sc
		DEALLOCATE sc


		UPDATE [DbSettings] SET [IntValue] = 8, [TimeValue] = GETDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 8, [TimeValue] = GETDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'DB upgrade complete.'
END
GO
