@ECHO OFF



IF EXIST _version\versionflag.cs GOTO test1ok
COPY /Y _version\versiona.cs _version\versionflag.cs >NUL



:test1ok
IF EXIST _version\versiongen.cs GOTO test2ok
_build\decout _version\versiongen.txt cmd /C ECHO 0 > _version\versiongen.cs



:test2ok
