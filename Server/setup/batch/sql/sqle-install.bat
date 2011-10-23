@ECHO OFF

IF -%1-==-silent- GOTO noprompt



ECHO ==========================================================================
ECHO   VR Estate Server Database setup
ECHO --------------------------------------------------------------------------
ECHO   This script shall setup required database on local MS SQL Express.
ECHO   SQL Express 2005 or above required.
ECHO.
ECHO   Make sure you have read notes in VR-Setup.sql file.
ECHO --------------------------------------------------------------------------
ECHO   Press any key to continue or [Ctrl-C] to exit.
PAUSE >NUL



:noprompt
sqlcmd -S .\SQLExpress -U sa -i vr-setup.sql



:quit
ECHO.
ECHO Press any key to exit.

IF -%1-==-silent- GOTO quit2
PAUSE >NUL
:quit2
