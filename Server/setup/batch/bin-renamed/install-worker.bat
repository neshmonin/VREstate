@ECHO OFF

SET TOOLPATH=%~dp0
SET ARCPATH=%~dp0\..\arc

SET INSTPATH=%PROGRAMFILES%\VREServer



IF -%1-==-silent- GOTO noprompt



ECHO ==========================================================================
ECHO   VR Estate Server Installer
ECHO --------------------------------------------------------------------------
ECHO   This script shall install/upgrade VR Estate Server on local machine.
ECHO   Install/upgrade path is "%INSTPATH%"
ECHO   System requirements:
ECHO   - Windows XP SP3, Windows 7, Windows 2008 R2 SP1
ECHO   - Microsoft .NET Framework 4.0 Full Profile
ECHO   - Windows XP SP2 Support Tools (Windows XP only)
ECHO --------------------------------------------------------------------------
ECHO   Press any key to continue or [Ctrl-C] to exit.
PAUSE >NUL



:noprompt

SET FOUND=
FOR %%X IN (httpcfg.exe) DO (SET FOUND=%%~$PATH:X)
IF DEFINED FOUND GOTO httpCfgFound

FOR %%X IN (netsh.exe) DO (SET FOUND=%%~$PATH:X)
IF DEFINED FOUND GOTO netshFound

ECHO HTTP configuration utility not found.
ECHO Make sure Windows XP SP2 Support Tools are installed and try again.
GOTO quit

:httpCfgFound
SET HTTPCFG=1
GOTO continue0

:netshFound
SET NETSH=1
GOTO continue0



:continue0

IF EXIST "%INSTPATH%\NUL" GOTO alreadyInstalled
IF EXIST "%INSTPATH%" GOTO alreadyInstalled
GOTO cleaninstall



:alreadyInstalled

CD /D "%INSTPATH%"

ECHO -------- Stopping service... --------

"%INSTDIR%\vreserver.exe" stop
IF ERRORLEVEL 1 GOTO error

GOTO continue1



:cleaninstall

ECHO -------- Creating programs folder... --------

MKDIR "%INSTPATH%"
IF ERRORLEVEL 1 GOTO errorPermission

CD /D "%INSTPATH%"

SET CLEAN=1



:continue1

ECHO -------- Extracting programs... --------
"%TOOLPATH%\7z.exe" x -aoa "%ARCPATH%\binaries.zip"
IF ERRORLEVEL 1 GOTO error

ECHO -------- Extracting configuration... --------
"%TOOLPATH%\7z.exe" x -aou "%ARCPATH%\defconfigs.zip"
IF ERRORLEVEL 1 GOTO error

IF DEFINED CLEAN GOTO noHttpSetup



ECHO -------- Setting up HTTP configuration... --------

IF DEFINED HTTPCFG GOTO doHttpCfg

netsh http add urlacl url=http://+:8026/vre/ user=\Everyone
IF ERRORLEVEL 1 GOTO error

rem netsh http add iplisten ipaddress=+:8026
rem IF ERRORLEVEL 1 GOTO error

GOTO continue2


:doHttpCfg

httpcfg set urlacl /u http://+:8026/vre/ /a "O:LAG:AUD:(A;;RPWPCCDCLCSWRCWDWOGA;;;S-1-1-0)"
IF ERRORLEVEL 1 GOTO error

httpcfg set iplisten -i +:8026
IF ERRORLEVEL 1 GOTO error

GOTO continue2



:continue2

ECHO -------- Starting service... --------

"%INSTDIR%\vreserver.exe" install
IF ERRORLEVEL 1 GOTO error

"%INSTDIR%\vreserver.exe" start
IF ERRORLEVEL 1 GOTO error



ECHO --------------------------------------------------------------------------
ECHO Installation complete.
ECHO Server is running.

GOTO quit



:noHttpSetup

ECHO --------------------------------------------------------------------------
ECHO Installation complete.
ECHO Please compare/merge updated configuration files (ending with _1.config)
ECHO   and start service manually. 

GOTO quit



:errorPermission
ECHO You seem not to have administrative rights on this machine.
ECHO Please log on as administrator and try again.
GOTO quit



:error
ECHO ERROR!
ECHO Installation aborted.



:quit
