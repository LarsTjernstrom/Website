@echo off
star --resourcedir="%~dp0src\Website\wwwroot" "%~dp0src\Website\bin\Debug\Website.exe"
star --resourcedir="%~dp0src\WebsiteProvider\wwwroot" "%~dp0src\WebsiteProvider\bin\Debug\WebsiteProvider.exe"
