ECHO ON

SET ADDONS0=C:\Program Files\7-Zip
SET ADDONS1=C:\Program Files\nHibernate\2.1.2\Required_For_LazyLoading\LinFu

MKDIR ready
CD ready
IF ERRORLEVEL 1 GOTO quit

DEL /S /Q *.*
RMDIR /S /Q .\*.*

XCOPY /f /s /e /r /h ..\batch\*.*
IF ERRORLEVEL 1 GOTO quit

REN bin-renamed bin
IF ERRORLEVEL 1 GOTO quit

REM MKDIR bin
REM CD bin
REM COPY "%ADDONS0%\7z.exe"
REM COPY "%ADDONS0%\7z.dll"

CD ..\..\Binaries

..\setup\ready\bin\7z.exe a ..\setup\ready\arc\binaries.zip -tzip -mx9 *.exe *.dll
..\setup\ready\bin\7z.exe a ..\setup\ready\arc\binaries.zip -tzip -mx9 "%ADDONS1%\*.dll"
IF ERRORLEVEL 1 GOTO quit

..\setup\ready\bin\7z.exe a ..\setup\ready\arc\defconfigs.zip -tzip -mx9 *.config -x!App.config -x!*.vshost.exe.config
IF ERRORLEVEL 1 GOTO quit

CD ..\setup\sql-parts

COPY header.txt+generated.sql+footer.txt ..\ready\sql\vr-setup.sql
IF ERRORLEVEL 1 GOTO quit

CD ..\ready
ECHO Done!


:quit