@ECHO OFF

PUSHD %~dp0src\WebsiteEditor
starpack -p
POPD

PUSHD %~dp0src\WebsiteProvider
starpack -p
POPD