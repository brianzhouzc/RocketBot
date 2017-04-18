@echo off
nuget.exe restore "RocketBot.sln"
set RocketBuilder=""
for /f "delims=" %%i in ('dir /s /b /a-d "%programfiles(x86)%\MSBuild.exe"') do (
if %RocketBuilder%=="" (set RocketBuilder="%%i")
)
%RocketBuilder% "RocketBot.sln" /property:Configuration="Release RocketBot" /property:Platform="Any CPU"
set RocketBuilder=