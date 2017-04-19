@echo off
nuget.exe restore "RocketBot.sln"
for /f "delims=" %%i in ('dir /s /b /a-d "%programfiles(x86)%\MSBuild.exe"') do (set RocketBuilder="%%i")
%RocketBuilder% "RocketBot.sln" /property:Configuration="Release RocketBot" /property:Platform="Any CPU"