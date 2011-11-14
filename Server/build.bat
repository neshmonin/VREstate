@ECHO OFF


SET devenv="C:\Program Files\Microsoft Visual Studio 10.0\Common7\IDE\devenv.exe"
SET git="C:\Program Files\GIT\bin\git.exe"



ECHO Checking repository status...
%git% diff --exit-code --quiet
IF ERRORLEVEL 1 GOTO dirty



ECHO Generating version stamp...
_build\decout _version\versiongen.txt %git% describe --always > _version\versiongen.cs
IF ERRORLEVEL 1 GOTO versionFailed



ECHO Building binaries...
%devenv% vreserver.sln /Rebuild Release /Out _build\build.log
IF ERRORLEVEL 1 GOTO devenvFailed



ECHO Packaging...

EXIT 0



:devenvFailed
ECHO ERROR: Building binaries failed; see errors in _build\build.log
EXIT 1



:versionFailed
ECHO ERROR: Version stamp tool failed.
EXIT 1



:dirty
ECHO ERROR: Repository is not clean.
ECHO Do a clean checkout or commit changed files before release.
EXIT 1
