--==========================================================================
--
-- VR Estate database upgrade script v27 -> v28
--
-- Script version 1
-- Last Modified on 2013-12-18 by Andrew
--
-- ==========================================================================
USE [VRT]
GO

DECLARE @ver INT
DECLARE @stage INT

SELECT @ver=[IntValue] FROM [DbSettings] WHERE [Id] = 3
SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @ver = 27 AND @stage = 0
BEGIN

	PRINT 'Schema upgrade...'

	BEGIN TRAN

		ALTER TABLE [dbo].[BrokerageInfo]
			ADD
				[Name] [nvarchar](256) NOT NULL DEFAULT '',
				[StreetAddress] [nvarchar](256) NOT NULL DEFAULT '',
				[CreditUnits] [money] NOT NULL DEFAULT 0,
				[LastServicePayment] [datetime] NOT NULL DEFAULT '1900-01-01'

		ALTER TABLE [dbo].[Users]
			ADD
				[PasswordChangeRequired] [bit] NOT NULL DEFAULT 0,
				[PhotoUrl] [nvarchar](256) NULL,
				[LastServicePayment] [datetime] NOT NULL DEFAULT '1900-01-01'

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


		UPDATE [dbo].[BrokerageInfo] SET [StreetAddress] = [AddressLine1], [Updated] = GETUTCDATE() WHERE [Deleted] = 0


		UPDATE [DbSettings] SET [IntValue] = 1, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Complete.'
END
GO



DECLARE @stage INT

SELECT @stage=[IntValue] FROM [DbSettings] WHERE [Id] = 4

IF @stage = 2
BEGIN
	PRINT 'Schema finalization...'
	BEGIN TRAN


		ALTER TABLE [dbo].[BrokerageInfo] DROP COLUMN [AddressLine1]
		ALTER TABLE [dbo].[BrokerageInfo] DROP COLUMN [AddressLine2]


		UPDATE [DbSettings] SET [IntValue] = 28, [TimeValue] = GETUTCDATE() WHERE [Id] = 1
		UPDATE [DbSettings] SET [IntValue] = 28, [TimeValue] = GETUTCDATE() WHERE [Id] = 3
		UPDATE [DbSettings] SET [IntValue] = 0, [TimeValue] = GETUTCDATE() WHERE [Id] = 4

	COMMIT TRAN
	PRINT 'Complete.'
END
GO
