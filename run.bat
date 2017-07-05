@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

star --resourcedir="%~dp0src\WebsiteEditor\wwwroot" "%~dp0src\WebsiteEditor\bin\%CONFIGURATION%\WebsiteEditor.exe"
star --resourcedir="%~dp0src\WebsiteProvider\wwwroot" "%~dp0src\WebsiteProvider\bin\%CONFIGURATION%\WebsiteProvider.exe"
