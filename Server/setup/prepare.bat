@ECHO OFF


CD /D "%~dp0"


IF NOT EXIST ready\NUL GOTO noready
RMDIR /S /Q ready
IF EXIST ready\NUL GOTO quit

:noready
MKDIR ready
CD ready
IF ERRORLEVEL 1 GOTO quit



XCOPY /f /s /e /r /h ..\batch\*.*
IF ERRORLEVEL 1 GOTO quit



REN bin-renamed bin
IF ERRORLEVEL 1 GOTO quit



CD ..\..\Binaries

..\setup\ready\bin\7z.exe a ..\setup\ready\arc\binaries.zip -tzip -mx9 *.exe *.dll
IF ERRORLEVEL 1 GOTO quit

..\setup\ready\bin\7z.exe a ..\setup\ready\arc\defconfigs.zip -tzip -mx9 *.config -x!App.config -x!*.vshost.exe.config
IF ERRORLEVEL 1 GOTO quit

..\setup\ready\bin\7z.exe a console.zip -tzip -mx9 vresrvcmd.exe vrebo.dll
IF ERRORLEVEL 1 GOTO quit
COPY /B ..\setup\console.png+console.zip ..\setup\ready\arc\console.png
IF ERRORLEVEL 1 GOTO quit

CD ..\setup\sql-parts

COPY header.txt+generated.sql+footer.txt ..\ready\sql\vr-setup.sql
IF ERRORLEVEL 1 GOTO quit

COPY vr-*-upgrade.sql ..\ready\sql\
IF ERRORLEVEL 1 GOTO quit


CD ..\ready
ECHO Done!


:quit