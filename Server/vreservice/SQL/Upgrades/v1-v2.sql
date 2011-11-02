-- This script upgrades default instance of VREstate DB from V1 to V2

USE [VR]
GO

DECLARE @ver INT

SELECT @ver = [IntValue] FROM [DbSettings] WHERE [Id] = 1

IF @ver = 1
BEGIN

	ALTER TABLE [SuiteLevels]
		ADD [Bedrooms] TINYINT NOT NULL DEFAULT (0)
		, [Dens] TINYINT NOT NULL DEFAULT (0)
		, [Toilets] TINYINT NOT NULL DEFAULT (0)
		, [Showers] TINYINT NOT NULL DEFAULT (0)
		, [Baths] TINYINT NOT NULL DEFAULT (0)
		, [Balconies] TINYINT NOT NULL DEFAULT (0)

	UPDATE [DbSettings] SET [IntValue] = 2 WHERE [Id] = 1
	UPDATE [DbSettings] SET [TimeValue] = GETUTCDATE() WHERE [Id] = 2

END

