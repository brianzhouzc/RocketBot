@echo off

nuget.exe restore "RocketBot.sln"
"C:\Program Files (x86)\MSBuild\14.0\Bin\MsBuild.exe" "RocketBot.sln" /property:Configuration="Release RocketBot" /property:Platform="Any CPU"
