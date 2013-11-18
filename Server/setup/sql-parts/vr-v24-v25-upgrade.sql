--==========================================================================
--
-- VR Estate database upgrade script v24 -> v25
--
-- Script version 1
-- Last Modified on 2013-11-14 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

CREATE FUNCTION NormalizeRelativePath (@link nvarchar(256))
RETURNS nvarchar(256)
AS
BEGIN
	DECLARE @ret AS nvarchar(256)

	IF LEN(@link) > 4 AND SUBSTRING(@link,1,4) = 'http' BEGIN
		SET @ret = replace(substring(@link,29,len(@link)-27),'\','/')
	END
	ELSE BEGIN
		SET @ret = replace(@link,'\','/')
	END
	RETURN @ret
END
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 24 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		CREATE TABLE [dbo].[FileStorageItem]
		(
			[AutoID] [int] IDENTITY(1,1) NOT NULL,
			[Version] [timestamp] NOT NULL,
			[Created] [datetime] NOT NULL,
			[Updated] [datetime] NOT NULL,
			[Deleted] [bit] NOT NULL,
			[Hash] [varbinary](128) NOT NULL,
			[Store] [int] NOT NULL,
			[RelativePath] [nvarchar](256) NOT NULL,
			[UseCounter] [int] NOT NULL,
			CONSTRAINT [PK__aFileStorageItem] PRIMARY KEY CLUSTERED 
			(
				[AutoID] ASC
			) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
		) ON [PRIMARY]

		CREATE NONCLUSTERED INDEX [IX_FileStorageItem_0] ON [dbo].[FileStorageItem]
		(
			[Store], [Hash], [Deleted]
		) WITH (STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


		

DECLARE @id AS int
DECLARE @link AS nvarchar(256)

DECLARE cc CURSOR FOR SELECT [autoid], [floorplanurl] FROM [SuiteTypes] WHERE [floorplanurl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [SuiteTypes] SET [Updated] = GETUTCDATE(), [floorplanurl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [WireframeLocation] FROM [Buildings] WHERE [WireframeLocation] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Buildings] SET [Updated] = GETUTCDATE(), [WireframeLocation] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [displaymodelurl] FROM [Buildings] WHERE [displaymodelurl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Buildings] SET [Updated] = GETUTCDATE(), [displaymodelurl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [overlaymodelurl] FROM [Buildings] WHERE [overlaymodelurl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Buildings] SET [Updated] = GETUTCDATE(), [overlaymodelurl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [poimodelurl] FROM [Buildings] WHERE [poimodelurl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Buildings] SET [Updated] = GETUTCDATE(), [poimodelurl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [BubbleWebTemplateUrl] FROM [Buildings] WHERE [BubbleWebTemplateUrl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Buildings] SET [Updated] = GETUTCDATE(), [BubbleWebTemplateUrl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [BubbleKioskTemplateUrl] FROM [Buildings] WHERE [BubbleKioskTemplateUrl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Buildings] SET [Updated] = GETUTCDATE(), [BubbleKioskTemplateUrl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [WireframeLocation] FROM [Sites] WHERE [WireframeLocation] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Sites] SET [Updated] = GETUTCDATE(), [WireframeLocation] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [displaymodelurl] FROM [Sites] WHERE [displaymodelurl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Sites] SET [Updated] = GETUTCDATE(), [displaymodelurl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [overlaymodelurl] FROM [Sites] WHERE [overlaymodelurl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Sites] SET [Updated] = GETUTCDATE(), [overlaymodelurl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [BubbleWebTemplateUrl] FROM [Sites] WHERE [BubbleWebTemplateUrl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Sites] SET [Updated] = GETUTCDATE(), [BubbleWebTemplateUrl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc

DECLARE cc CURSOR FOR SELECT [autoid], [BubbleKioskTemplateUrl] FROM [Sites] WHERE [BubbleKioskTemplateUrl] IS NOT NULL
OPEN cc
FETCH NEXT FROM cc INTO @id, @link
WHILE @@FETCH_STATUS = 0 BEGIN
	UPDATE [Sites] SET [Updated] = GETUTCDATE(), [BubbleKioskTemplateUrl] = dbo.NormalizeRelativePath(@link)
		WHERE [autoid] = @id
	FETCH NEXT FROM cc INTO @id, @link
END
CLOSE cc
DEALLOCATE cc



		UPDATE [DbSettings] SET [IntValue] = 21, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 25, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4



	COMMIT TRAN
	PRINT 'Complete.'
END
GO
