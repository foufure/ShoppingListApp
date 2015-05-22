@ECHO OFF
"..\..\..\packages\OpenCover.4.5.3723\OpenCover.Console.exe" -log:Verbose -target:"..\..\..\packages\NUnit.Runners.2.6.4\tools\nunit-console.exe" -targetargs:"ShoppingListApp.Web.UI.Test.dll /noshadow /nologo /framework:net-4.5.1" -register:"user" -mergebyhash -output:"..\..\..\coverage\ShoppingListApp.Web.UI.Test.xml"
pause
if %ERRORLEVEL% GEQ 1 GOTO ERROR

echo starting reportgenerator
REM "..\..\..\packages\ReportGenerator.2.1.4.0\reportgenerator.exe" -reports:"..\..\..\coverage\ShoppingListApp.Web.UI.Test.xml" -targetdir:"..\..\..\coverage" Verbosity:"Error"
pause
if %ERRORLEVEL% GEQ 1 GOTO ERROR

echo starting Picles report
pause
REM "..\..\..\packages\Pickles.CommandLine.1.1.0\tools\pickles.exe" --feature-directory="..\..\..\ShoppingListApp.Web.UI.Tests\Features" --output-directory="..\..\..\pickles"
if %ERRORLEVEL% GEQ 1 GOTO ERROR

REM Firefox is causing the batch file to return an error code 1
REM "C:\Program Files (x86)\Mozilla Firefox\firefox.exe" "..\..\..\coverage\index.htm"
pause
exit 0

:ERROR
pause
exit 0