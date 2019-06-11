setlocal
set specFile=%1
set namespace=%2
set generateFolder=%3

set repoRoot=%~dp0..

@echo on
autorest -Modeler Swagger -CodeGenerator CSharp -Namespace %namespace% -ClientName %namespace% -Input %specFile% -outputDirectory %generateFolder% -Header "Copyright (c) Microsoft Corporation.  All rights reserved." %~5
@echo off
endlocal
