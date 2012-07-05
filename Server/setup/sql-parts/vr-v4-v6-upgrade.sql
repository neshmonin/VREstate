--==========================================================================
--
-- VR Estate database upgrade script v4 -> v6
--
-- Adds
--
-- Script version 1
-- Last Modified on 2012-07-02 by Andrew
--
-- ==========================================================================
USE [VRT]
GO


DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 4 
BEGIN

	PRINT 'Preliminary schema upgrade...'

	BEGIN TRAN

		-- ----------------------------------------------------------------------
		-- Creating new tables
		-- ----------------------------------------------------------------------

		--
		-- Brokerage info
		--
		CREATE TABLE [dbo].[BrokerageInfo](
			[AutoID] [int] IDENTITY(1,1) NOT NULL,
			[Version] [timestamp] NOT NULL,
			[Emails] [nvarchar](max) NOT NULL,
			[PhoneNumbers] [nvarchar](max) NOT NULL,
			[AddressLine1] [nvarchar](128) NULL,
			[AddressLine2] [nvarchar](128) NULL,
			[City] [nvarchar](64) NULL,
			[StateProvince] [nvarchar](8) NULL,
			[PostalCode] [nvarchar](10) NULL,
			[Country] [nvarchar](128) NULL,
			[Web] [nvarchar](256) NULL,
			[Logo] [nvarchar](256) NULL
			CONSTRAINT [PK_BrokerageInfo] PRIMARY KEY CLUSTERED 
			(
				[AutoID] ASC
			)
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
			ON [PRIMARY]
		) ON [PRIMARY]
		
		--
		-- Licensing
		--
		CREATE TABLE [dbo].[UserLicenses](
			[AutoID] [int] IDENTITY(1,1) NOT NULL,
			[Version] [timestamp] NOT NULL,
			[Created] [datetime] NOT NULL,
			[Updated] [datetime] NOT NULL,
			[Deleted] [bit] NOT NULL,
			[ExpiryTime] [datetime] NOT NULL,
			[LicenseeID] [int] NOT NULL,
			[UpdaterID] [int] NOT NULL,
			[SiteID] [int] NOT NULL
			CONSTRAINT [PK_UserLicences] PRIMARY KEY CLUSTERED 
			(
				[AutoID] ASC
			)
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
			ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[UserLicenses] 
			WITH CHECK 
			ADD CONSTRAINT [FK_Licenses_Updater] FOREIGN KEY([UpdaterID]) REFERENCES [dbo].[Users] ([AutoID])
		ALTER TABLE [dbo].[UserLicenses] 
			CHECK CONSTRAINT [FK_Licenses_Updater]

		ALTER TABLE [dbo].[UserLicenses] 
			WITH CHECK 
			ADD CONSTRAINT [FK_Licenses_Licensee] FOREIGN KEY([LicenseeID]) REFERENCES [dbo].[Users] ([AutoID])
		ALTER TABLE [dbo].[UserLicenses] 
			CHECK CONSTRAINT [FK_Licenses_Licensee]

		ALTER TABLE [dbo].[UserLicenses] 
			WITH CHECK 
			ADD CONSTRAINT [FK_Licenses_Site] FOREIGN KEY([SiteID]) REFERENCES [dbo].[Sites] ([AutoID])
		ALTER TABLE [dbo].[UserLicenses] 
			CHECK CONSTRAINT [FK_Licenses_Site]

		--
		-- User-user access granting xref
		--
		CREATE TABLE [dbo].[UserUserAccessMM](
			[FromID] [int] NOT NULL,
			[ToID] [int] NOT NULL,
			CONSTRAINT [PK_UserUserMM] PRIMARY KEY CLUSTERED 
			(
				[FromID] ASC,
				[ToID] ASC
			)
			WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) 
			ON [PRIMARY]
		) ON [PRIMARY]

		ALTER TABLE [dbo].[UserUserAccessMM]  
			WITH CHECK 
			ADD CONSTRAINT [FK_UserUserAccess] FOREIGN KEY([FromID]) REFERENCES [dbo].[Users] ([AutoID])
		ALTER TABLE [dbo].[UserUserAccessMM] 
			CHECK CONSTRAINT [FK_UserUserAccess]

		ALTER TABLE [dbo].[UserUserAccessMM]
			WITH CHECK
			ADD CONSTRAINT [FK_UserUserAccess1] FOREIGN KEY([ToID])	REFERENCES [dbo].[Users] ([AutoID])
		ALTER TABLE [dbo].[UserUserAccessMM] 
			CHECK CONSTRAINT [FK_UserUserAccess1]

		-- ----------------------------------------------------------------------
		-- Pre-modifying existing tables
		-- ----------------------------------------------------------------------

		ALTER TABLE [dbo].[Buildings]
			ADD
				[InitialView] [nvarchar](max) NULL,
				[SellerID] [int] NULL

		ALTER TABLE [dbo].[Buildings]
			WITH CHECK
			ADD CONSTRAINT [FK_BuildingSeller] FOREIGN KEY([SellerID]) REFERENCES [dbo].[Users] ([AutoID])
		ALTER TABLE [dbo].[Buildings] 
			CHECK CONSTRAINT [FK_BuildingSeller]

		ALTER TABLE [dbo].[Sites]
			ADD
				[InitialView] [nvarchar](max) NULL

		ALTER TABLE [dbo].[ContactInfo]
			ADD
				[Photo] [nvarchar](256) NULL

		ALTER TABLE [dbo].[Suites]
			ADD
				[SellerID] [int] NULL

		ALTER TABLE [dbo].[Suites]
			WITH CHECK
			ADD CONSTRAINT [FK_SuiteSeller] FOREIGN KEY([SellerID])	REFERENCES [dbo].[Users] ([AutoID])
		ALTER TABLE [dbo].[Suites] 
			CHECK CONSTRAINT [FK_SuiteSeller]
			
		ALTER TABLE [dbo].[Users]
			ADD
				[BrokerInfoID] [int] NULL,
				[LastLogin] [datetime] NULL
			
		ALTER TABLE [dbo].[Users]
			WITH CHECK
			ADD CONSTRAINT [FK_UserBrokerageInfo] FOREIGN KEY([BrokerInfoID]) REFERENCES [dbo].[BrokerageInfo] ([AutoID])
		ALTER TABLE [dbo].[Users] 
			CHECK CONSTRAINT [FK_UserBrokerageInfo]

		UPDATE [DbSettings] SET [IntValue] = -1 WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = -1 WHERE [Id] = 3
		INSERT INTO [DbSettings] ([Id], [IntValue], [TimeValue]) VALUES (4, 1, GETDATE())

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

		UPDATE [dbo].[Buildings] SET [InitialView] = '' WHERE [InitialView] IS NULL
		UPDATE [dbo].[Sites] SET [InitialView] = '' WHERE [InitialView] IS NULL
		UPDATE [dbo].[Users] SET [LastLogin] = GETDATE() WHERE [LastLogin] IS NULL

		UPDATE [DbSettings] SET [IntValue] = 2 WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Data transfers complete.'
END
GO



DECLARE @stage INT

SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @stage = 2
BEGIN
	PRINT 'Finalizing schema changes...'
	BEGIN TRAN	

		ALTER TABLE [dbo].[Buildings] 
			ALTER COLUMN [InitialView] [nvarchar](max) NOT NULL

		ALTER TABLE [dbo].[Sites] 
			ALTER COLUMN [InitialView] [nvarchar](max) NOT NULL

		ALTER TABLE [dbo].[Users] 
			ALTER COLUMN [LastLogin] [datetime] NOT NULL

		UPDATE [DbSettings] SET [IntValue] = 6, [TimeValue] = GETDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 6, [TimeValue] = GETDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETDATE() WHERE [Id] = 4
		
	COMMIT TRAN
	PRINT 'DB upgrade complete.'
END