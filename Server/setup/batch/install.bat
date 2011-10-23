@ECHO OFF

CALL bin\install-worker.bat %1

IF -%1-==-silent- GOTO quit
ECHO.
ECHO Press any key to exit.
PAUSE >NUL

:quit
