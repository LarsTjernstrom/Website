@ECHO OFF

:: Set up the env to use Msbuild 14.0
CALL "%VS140COMNTOOLS%\vsvars32.bat"

msbuild
POPD