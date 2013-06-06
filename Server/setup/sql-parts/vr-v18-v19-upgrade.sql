--==========================================================================
--
-- VR Estate database upgrade script v18 -> v19
--
-- Script version 1
-- Last Modified on 2013-06-04 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 18 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		CREATE TABLE [dbo].[NamedSearchFilters]
		(
			[AutoID] [int] IDENTITY(1,1) NOT NULL,
			[Version] [timestamp] NOT NULL,
			[Created] [datetime] NOT NULL,
			[Updated] [datetime] NOT NULL,
			[Deleted] [bit] NOT NULL,
			[OwnerId] [int] NOT NULL,
			[RelatedUserId] [int] NULL,
			[Note] [nvarchar](MAX) NULL,
			[Filter] [nvarchar](MAX) NULL,
			[ViewPointInfo] [nvarchar](MAX) NULL,
			CONSTRAINT [PK__aVrNamedSearchFilters] PRIMARY KEY CLUSTERED 
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
	PRINT 'Adding constraints...'
	BEGIN TRAN

		-- ----------------------------------------------------------------------
		-- Adding constraints
		-- ----------------------------------------------------------------------

		ALTER TABLE [dbo].[NamedSearchFilters] WITH CHECK ADD CONSTRAINT [FK_NamedSearchFilters_Users] FOREIGN KEY([OwnerId])
			REFERENCES [dbo].[Users] ([AutoID])

		ALTER TABLE [dbo].[NamedSearchFilters] CHECK CONSTRAINT [FK_NamedSearchFilters_Users]

		UPDATE [DbSettings] SET [IntValue] = 19, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 19, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Complete.'
END
GO
