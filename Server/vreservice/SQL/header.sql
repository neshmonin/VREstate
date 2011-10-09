--===========================================================================
-- This script shall create database for VRE server.
--===========================================================================

PRINT 'Checking prerequisites...'

USE Master

IF EXISTS (SELECT * FROM sys.databases WHERE [name] = 'VR')
BEGIN
	PRINT 'ERROR: VR database exists; please drop it manually before running this script.'
	RETURN
END

IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE [name] = 'vr')
BEGIN
	PRINT 'ERROR: ''vr'' login does not exist.'
	RETURN
END



PRINT 'Creating database...'

CREATE DATABASE VR
GO
USE VR



PRINT 'Setting up access...'

EXEC sp_grantdbaccess 'vr', 'vr'
EXEC sp_addrolemember 'db_owner', 'vr'



PRINT 'Creating tables...'

-----------------------------------------------------------------------------
