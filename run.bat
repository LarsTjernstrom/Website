@ECHO OFF

IF "%CONFIGURATION%"=="" SET CONFIGURATION=Debug

star %* --resourcedir="%~dp0src\Website\wwwroot" "%~dp0src\Website\bin\%CONFIGURATION%\Website.exe"
star %* --resourcedir="%~dp0src\WebsiteProvider\wwwroot" "%~dp0src\WebsiteProvider\bin\%CONFIGURATION%\WebsiteProvider.exe"
