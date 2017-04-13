@echo off
nuget.exe restore "RocketBot.sln"
"C:\Program Files (x86)\Microsoft Visual Studio\2017\BuildTools\MSBuild\15.0\Bin\MsBuild.exe" "RocketBot.sln" /property:Configuration="Release RocketBot" /property:Platform="Any CPU"
