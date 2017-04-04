setlocal
set specFile=%1
set namespace=%2
set autoRestVersion=%3
set generateFolder=%4

set source=-Source https://www.myget.org/F/autorest/api/v2

set repoRoot=%~dp0..
set autoRestExe=%repoRoot%\packages\AutoRest.%autoRestVersion%\tools\AutoRest.exe

powershell -command "& { (New-Object Net.WebClient).DownloadFile('https://dist.nuget.org/win-x86-commandline/latest/nuget.exe', '%repoRoot%\scripts\tools\nuget.exe') }"
%repoRoot%\scripts\tools\nuget.exe install AutoRest %source% -Version %autoRestVersion% -o %repoRoot%\packages -verbosity quiet

@echo on
%autoRestExe% -Modeler Swagger -CodeGenerator CSharp -Namespace %namespace% -ClientName %namespace% -Input %specFile% -outputDirectory %generateFolder% -Header "Copyright (c) Microsoft Corporation.  All rights reserved." %~5
@echo off
endlocal
