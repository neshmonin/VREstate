-----------------------------------------------------------------------------

PRINT 'Setting up database...'

-- This value MUST equal Vre.Server.DatabaseSettings.CurrentDbVersion value in server code.
INSERT INTO [DbSettings] ([Id], [IntValue]) VALUES (1, 1)

INSERT INTO [DbSettings] ([Id], [TimeValue]) VALUES (2, GETUTCDATE())

-----------------------------------------------------------------------------

PRINT 'Script complete.  Version 1.'
