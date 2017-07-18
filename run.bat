@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

REM star --resourcedir="%~dp0src\WebsiteEditor\wwwroot" "%~dp0src\WebsiteEditor\bin\%CONFIGURATION%\WebsiteEditor.exe"
REM star --resourcedir="%~dp0src\WebsiteProvider\wwwroot" "%~dp0src\WebsiteProvider\bin\%CONFIGURATION%\WebsiteProvider.exe"
star --resourcedir="%~dp0test\WebsiteProvider_AcceptanceHelperOne\wwwroot" "%~dp0test/WebsiteProvider_AcceptanceHelperOne/bin/%Configuration%/WebsiteProvider_AcceptanceHelperOne.exe"
star --resourcedir="%~dp0test\WebsiteProvider_AcceptanceHelperTwo\wwwroot" "%~dp0test/WebsiteProvider_AcceptanceHelperTwo/bin/%Configuration%/WebsiteProvider_AcceptanceHelperTwo.exe"
