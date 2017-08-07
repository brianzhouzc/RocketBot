@echo off
echo Build preparation....
cd %CD%
echo Select you platform (5 secs - default platform = Any CPU)
echo.
echo 1 = Any CPU 
echo 2 = x86
echo 3 = x64
CHOICE /C 123 /N /T 5 /D 1 /M "Select you build:"
IF ERRORLEVEL 1 SET Platform=Any CPU
IF ERRORLEVEL 2 SET Platform=x86
IF ERRORLEVEL 3 SET Platform=x64
ECHO You chose %Platform%
echo.
echo Select you configuration (5 secs - default configuration = Release)
echo.
echo 1 = Release
echo 2 = Debug
CHOICE /C 12 /N /T 5 /D 1 /M "Select you build configuration:"
IF ERRORLEVEL 1 SET Release=Release
IF ERRORLEVEL 2 SET Release=Debug
ECHO You chose %Release%
echo.
echo Build initialised %Release% - %Platform%
echo.
nuget.exe restore "RocketBot.sln"
for /f "delims=" %%i in ('dir /s /b /a-d "%programfiles(x86)%\MSBuild.exe"') do (set RocketBuilder="%%i")
%RocketBuilder% "RocketBot.sln" /property:Configuration="%Release%" /property:Platform="%Platform%"
set RocketBuilder=
pause
