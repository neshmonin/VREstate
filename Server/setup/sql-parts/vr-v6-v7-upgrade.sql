--==========================================================================
--
-- VR Estate database upgrade script v6 -> v7
--
-- Adds
--
-- Script version 1
-- Last Modified on 2012-10-09 by Andrew
--
-- ==========================================================================
USE [VRA]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 6
BEGIN

	PRINT 'Preliminary schema upgrade...'

	BEGIN TRAN

		-- ----------------------------------------------------------------------
		-- Pre-modifying existing tables
		-- ----------------------------------------------------------------------

		ALTER TABLE [dbo].[BrokerageInfo]
			ADD
				[Created] [datetime] NOT NULL DEFAULT GETDATE(),
				[Updated] [datetime] NOT NULL DEFAULT GETDATE(),
				[Deleted] [bit] NOT NULL DEFAULT 0

		ALTER TABLE [dbo].[Buildings]
			ADD
				[OverlayModelUrl] [nvarchar](256) NULL,
				[BubbleTemplateUrl] [nvarchar](256) NULL,
				[WireframeLocation] [nvarchar](256) NULL

		EXEC sp_rename 'dbo.Buildings.Model', 'DisplayModelUrl', 'COLUMN'


		ALTER TABLE [dbo].[Sites]
			ADD
				[OverlayModelUrl] [nvarchar](256) NULL,
				[BubbleTemplateUrl] [nvarchar](256) NULL,
				[WireframeLocation] [nvarchar](256) NULL

		EXEC sp_rename 'dbo.Sites.GenericInfoModel', 'DisplayModelUrl', 'COLUMN'


		ALTER TABLE [dbo].[Suites]
			ADD
				[BubbleTemplateUrl] [nvarchar](256) NULL

		ALTER TABLE [dbo].[SuiteTypes]
			ADD
				[FloorPlanUrl] [nvarchar](256) NULL

		ALTER TABLE [dbo].[Users]
			ADD
				[NickName] [nvarchar](256) NULL,
				[PrimaryEmailAddress] [nvarchar](256) NULL,
				[TimeZone] [varchar](256) NULL,
				[PersonalInfo] [nvarchar](max) NULL

		-- ----------------------------------------------------------------------
		-- Creating new tables
		-- ----------------------------------------------------------------------

		CREATE TABLE [dbo].[Listings] (
			[AutoID] [uniqueidentifier] NOT NULL,
			[Version] [timestamp] NOT NULL,
			[Created] [datetime] NOT NULL,
			[Updated] [datetime] NOT NULL,
			[Deleted] [bit] NOT NULL,
			[OwnerId] [int] NOT NULL,
			[ExpiresOn] [datetime] NOT NULL,
			[PaymentRefId] [nvarchar](1024) NOT NULL,
			[RequestCounter] [int] NOT NULL,
			[LastRequestTime] [datetime] NOT NULL,
			[Product] [int] NOT NULL,
			[ProductUrl] [nvarchar](max) NULL,
			[MlsId] [nvarchar](256) NULL,
			[TargetObjectType] [int] NOT NULL,
			[TargetObjectId] [int] NOT NULL,
			CONSTRAINT [PK_Listings] PRIMARY KEY CLUSTERED ( [AutoID] ASC )
				WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON)
				ON [PRIMARY]
			) ON [PRIMARY]

		CREATE TABLE [dbo].[ReverseRequests] (
			[ID] [uniqueidentifier] NOT NULL,
			[Request] [int] NOT NULL,
			[ExpiresOn] [datetime] NOT NULL,
			[UserId] [int] NULL,
			[Login] [nvarchar](64) NULL,
			[Subject] [nvarchar](max) NULL,
			[ReferenceParamName] [nvarchar](128) NULL,
			[ReferenceParamValue] [nvarchar](128) NULL
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


DECLARE @vrroot varchar(256)
DECLARE @vrrel varchar(256)
SET @vrroot = 'C:\vr.alternative\www\'
SET @vrrel = 'SuperServer\'



DECLARE @stage INT

SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @stage = 1
BEGIN
	PRINT 'Data transfers...'
	BEGIN TRAN

		-- ----------------------------------------------------------------------
		-- Fixing data
		-- ----------------------------------------------------------------------

		DECLARE @rpath varchar(1024)
		DECLARE @rfile varchar(1024)
		DECLARE @path varchar(1024)
		DECLARE @exists int
		DECLARE @eid int
		DECLARE @sid int
		DECLARE @id int
		DECLARE @name nvarchar(256)

		DECLARE bldg CURSOR FOR SELECT [EstateDeveloperID] AS [eid], [AutoID] AS [sid] FROM [dbo].[Sites] WHERE [Deleted] = 0
		OPEN bldg

		FETCH NEXT FROM bldg INTO @eid, @sid
		WHILE @@FETCH_STATUS = 0 BEGIN

			SET @rpath = @vrrel + CAST(@eid AS varchar(4)) + '\' + CAST(@sid AS varchar(4)) + '\'

			PRINT @vrroot + @rpath + '...'

			SET @rfile = @rpath + 'Model.kmz'
			SET @path = @vrroot + @rfile
			EXEC master.dbo.xp_fileexist @path, @exists OUTPUT
			IF @exists = 1 BEGIN
				UPDATE [dbo].[Sites]
					SET [DisplayModelUrl] = REPLACE(@rfile, '\', '/'), [WireframeLocation] = @rfile
					WHERE [AutoID] = @sid
				PRINT '... model found.'
			END
			
			SET @rfile = @rpath + 'Overlay.kmz'
			SET @path = @vrroot + @rfile
			EXEC master.dbo.xp_fileexist @path, @exists OUTPUT
			IF @exists = 1 BEGIN
				UPDATE [dbo].[Sites]
					SET [OverlayModelUrl] = REPLACE(@rfile, '\', '/')
					WHERE [AutoID] = @sid
				PRINT '... overlay found.'
			END
			

			FETCH NEXT FROM bldg INTO @eid, @sid
		END

		CLOSE bldg
		DEALLOCATE bldg
		
		DECLARE sttp CURSOR FOR SELECT s.[EstateDeveloperID] AS [eid], st.[SiteID] as [sid], st.[AutoID], st.[Name]
			FROM [dbo].[SuiteTypes] st
			INNER JOIN [dbo].[Sites] s ON s.[AutoID] = st.[SiteID]
			WHERE st.[Deleted] = 0
		OPEN sttp

		FETCH NEXT FROM sttp INTO @eid, @sid, @id, @name
		WHILE @@FETCH_STATUS = 0 BEGIN
			
			SET @rfile = @vrrel + CAST(@eid AS varchar(4)) + '\' + CAST(@sid AS varchar(4)) + '\SuitesWeb\' + CAST(@name AS varchar(256)) + '.html'

			SET @path = @vrroot + @rfile
			EXEC master.dbo.xp_fileexist @path, @exists OUTPUT
			IF @exists = 1 BEGIN
				UPDATE [dbo].[SuiteTypes]
					SET [FloorPlanUrl] = REPLACE(@rfile, '\', '/')
					WHERE [AutoID] = @id
				PRINT 'Floor plan: ' + @path
			END

			FETCH NEXT FROM sttp INTO @eid, @sid, @id, @name
		END

		CLOSE sttp
		DEALLOCATE sttp
		


		UPDATE [DbSettings] SET [IntValue] = 7, [TimeValue] = GETDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 7, [TimeValue] = GETDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'DB upgrade complete.'
END
GO
