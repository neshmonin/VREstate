@ECHO OFF


SET devenv="C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe"
SET git="C:\Program Files\GIT\bin\git.exe"



ECHO Checking repository status...
%git% diff --exit-code --quiet
IF ERRORLEVEL 1 GOTO dirty

COPY /Y _version\versionp.cs _version\versionflag.cs >NUL
GOTO next0

:dirty
COPY /Y _version\versiona.cs _version\versionflag.cs >NUL



:next0
ECHO Generating version stamp...
_build\decout _version\versiongen.txt %git% describe --always > _version\versiongen.cs
IF ERRORLEVEL 1 GOTO versionFailed



ECHO Building binaries...
DEL _build\build.log
%devenv% vreserver.sln /Rebuild Release /Out _build\build.log
IF ERRORLEVEL 1 GOTO devenvFailed



ECHO Packaging...
CALL setup\prepare.bat
IF ERRORLEVEL 1 GOTO packagingFailed
PAUSE
EXIT 0



:packagingFailed
ECHO ERROR: Packaging failed.
PAUSE
EXIT 1



:devenvFailed
ECHO ERROR: Building binaries failed; see errors in _build\build.log
PAUSE
EXIT 1



:versionFailed
ECHO ERROR: Version stamp tool failed.
PAUSE
EXIT 1



:dirty
ECHO ERROR: Repository is not clean.
ECHO Do a clean checkout or commit changed files before release.
PAUSE
EXIT 1
