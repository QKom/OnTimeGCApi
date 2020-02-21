@echo off
set SIGN=true
set NAME=OnTimeGCApi
set PATHTOFILE=bin\Release\netstandard2.0\OnTimeGCApi.dll
set BUILDPATH=bin\Release

rmdir /S /Q %BUILDPATH%
msbuild %NAME%.sln /t:rebuild /p:Configuration=Release
FOR /F "tokens=*" %%a in ('AssemblyVersion.exe "%PATHTOFILE%"') do SET VERSION=%%a

if "%SIGN%" == "true" (
    call "D:\workspace\dev\ShellScripte\CodeSigning.bat" "%PATHTOFILE%"
)

nuget pack %NAME%.nuspec -Version %VERSION%
nuget add %NAME%.%VERSION%.nupkg -Source \\samba.host.0x59.de\sonstiges\Nuget.Packages
del %NAME%.%VERSION%.nupkg
pause