@echo off
SETLOCAL

REM Create empty folder where will will create the file structure to be packed (zipped)
IF EXIST "%~dp0temp\NUL" (
	rd "%~dp0temp" /S /Q
) else (
 md "%~dp0temp"
)

REM Prepare Executables
md "%~dp0temp\app"
xcopy "%~dp0bin\Debug\Website.exe" "%~dp0temp\app"
xcopy "%~dp0bin\Debug\Website.pdb" "%~dp0temp\app"

REM Prepare wwwroot
md "%~dp0temp\wwwroot"
xcopy "%~dp0Website\Website\wwwroot" "%~dp0temp\wwwroot" /s /e

REM Copy icon and config
xcopy "%~dp0Website\Website\package\*.png" "%~dp0temp"
xcopy "%~dp0Website\Website\package\*.config" "%~dp0temp"

REM Get folder name for the zip name
for %%a in ("%~dp0.") do set currentfolder=%%~na


IF NOT EXIST "%~dp0packages" (
 md "%~dp0packages"
)

REM Zipp-it
CD "%~dp0temp"

zip -r "..\packages\Website.zip" *.*  || (GOTO ERROR)

CD "%~dp0"
rd "%~dp0temp" /S /Q

REM Create empty folder where will will create the file structure to be packed (zipped)
IF EXIST "%~dp0temp\NUL" (
	rd "%~dp0temp" /S /Q
) else (
 md "%~dp0temp"
)

REM Prepare Executables
md "%~dp0temp\app"
xcopy "%~dp0bin\Debug\Content.exe" "%~dp0temp\app"
xcopy "%~dp0bin\Debug\Content.pdb" "%~dp0temp\app"

REM Prepare wwwroot
md "%~dp0temp\wwwroot"
xcopy "%~dp0Website\Content\wwwroot" "%~dp0temp\wwwroot" /s /e

REM Copy icon and config
xcopy "%~dp0Website\Content\package\*.png" "%~dp0temp"
xcopy "%~dp0Website\Content\package\*.config" "%~dp0temp"

REM Get folder name for the zip name
for %%a in ("%~dp0.") do set currentfolder=%%~na


IF NOT EXIST "%~dp0packages" (
 md "%~dp0packages"
)

REM Zipp-it
CD "%~dp0temp"

zip -r "..\packages\Content.zip" *.*  || (GOTO ERROR)

REM TODO Jump back to previous dir
GOTO :CLEANUP

:ERROR

:CLEANUP
CD "%~dp0"
rd "%~dp0temp" /S /Q

:END

ENDLOCAL
