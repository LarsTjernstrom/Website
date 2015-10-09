@echo off
staradmin kill all

REM Start Website
call "%~dp0..\Website\run.bat"

call "%~dp0run.bat"