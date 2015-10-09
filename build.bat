@echo off

REM Echo Getting Source from GIT
rem call git --git-dir="%~dp0.git" pull || (GOTO ERROR)

REM call git --git-dir="%~dp0.git" --work-tree="%~dp0" pull && (echo success) || (GOTO ERROR)
SET FOUND_SOURCE=0
REM Echo Building Source
FOR %%i IN (%~dp0Website\Website\*.csproj) DO ( 
SET FOUND_SOURCE=1
REM msbuild %~dp0\Website\Website\Website.csproj && (GOTO ERROR)
msbuild %%i || (GOTO ERROR)
)

IF "%FOUND_SOURCE%"=="0" ( echo No source found, check batch argument
GOTO ERROR  )

SET FOUND_SOURCE=0
REM Echo Building Source
FOR %%i IN (%~dp0Website\Content\*.csproj) DO ( 
SET FOUND_SOURCE=1
REM msbuild %~dp0\Website\Content\Content.csproj && (GOTO ERROR)
msbuild %%i || (GOTO ERROR)
)

IF "%FOUND_SOURCE%"=="0" ( echo No source found, check batch argument
GOTO ERROR  )

GOTO END

:ERROR
echo Error building!

:END