@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

SET INTERACTIVE=1
ECHO %CMDCMDLINE% | find /i "%~0" >nul
IF NOT ERRORLEVEL 1 SET INTERACTIVE=0

:: Stop all the running apps
staradmin stop db default

:: Start the tested apps
star --resourcedir="%~dp0src\WebsiteEditor\wwwroot" "%~dp0src/WebsiteEditor/bin/%Configuration%/WebsiteEditor.exe"
IF ERRORLEVEL 1 EXIT /B 1

star --resourcedir="%~dp0src\WebsiteProvider\wwwroot" "%~dp0src/WebsiteProvider/bin/%Configuration%/WebsiteProvider.exe"
IF ERRORLEVEL 1 EXIT /B 1

:: Start the helper apps
star --resourcedir="%~dp0test\WebsiteProvider_AcceptanceHelperOne\wwwroot" "%~dp0test/WebsiteProvider_AcceptanceHelperOne/bin/%Configuration%/WebsiteProvider_AcceptanceHelperOne.exe"
IF ERRORLEVEL 1 EXIT /B 1

star --resourcedir="%~dp0test\WebsiteProvider_AcceptanceHelperTwo\wwwroot" "%~dp0test/WebsiteProvider_AcceptanceHelperTwo/bin/%Configuration%/WebsiteProvider_AcceptanceHelperTwo.exe"
IF ERRORLEVEL 1 EXIT /B 1

:: Start the test
IF NOT EXIST "%~dp0packages\NUnit.ConsoleRunner.3.6.1\" (ECHO Error: Cannot find NUnit Console Runner. Build the project to restore the packages && PAUSE && EXIT /B 1)
%~dp0packages\NUnit.ConsoleRunner.3.6.1\tools\nunit3-console.exe %~dp0test\WebsiteProvider.Tests\bin\Debug\WebsiteProvider.Tests.dll --noheader

:: If we are in interactive mode (batch file not started from the command line), pause at the exit
if %INTERACTIVE%==0 PAUSE

IF ERRORLEVEL 1 EXIT /B 1