@ECHO OFF

PUSHD %~dp0src\Website
starpack -p
POPD

PUSHD %~dp0src\WebsiteProvider
starpack -p
POPD