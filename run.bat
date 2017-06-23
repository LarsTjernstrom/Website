@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

REM star --resourcedir="%~dp0src\Website\wwwroot" "%~dp0src\Website\bin\%CONFIGURATION%\Website.exe"
REM star --resourcedir="%~dp0src\WebsiteProvider\wwwroot" "%~dp0src\WebsiteProvider\bin\%CONFIGURATION%\WebsiteProvider.exe"
star --resourcedir="%~dp0test\WebsiteProvider_AcceptanceHelperOne\wwwroot" "%~dp0test/WebsiteProvider_AcceptanceHelperOne/bin/%Configuration%/WebsiteProvider_AcceptanceHelperOne.exe"
star --resourcedir="%~dp0test\WebsiteProvider_AcceptanceHelperTwo\wwwroot" "%~dp0test/WebsiteProvider_AcceptanceHelperTwo/bin/%Configuration%/WebsiteProvider_AcceptanceHelperTwo.exe"
