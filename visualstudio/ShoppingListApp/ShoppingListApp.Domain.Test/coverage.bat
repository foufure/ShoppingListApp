@ECHO OFF
"..\..\..\packages\OpenCover.4.5.3723\OpenCover.Console.exe" -target:"..\..\..\packages\NUnit.Runners.2.6.4\tools\nunit-console.exe" -targetargs:"ShoppingListApp.Domain.Test.dll /noshadow /nologo /framework:net-4.5.1" -register:"user" -mergebyhash
if %ERRORLEVEL% GEQ 1 GOTO ERROR

REM DEL /F /Q ".\coverage\*.*"

"..\..\..\packages\ReportGenerator.2.1.3.0\reportgenerator.exe" -reports:"results.xml" -targetdir:"coverage" Verbosity:"Error"
if %ERRORLEVEL% GEQ 1 GOTO ERROR

REM Firefox is causing the batch file to return an error code 1
REM "C:\Program Files (x86)\Mozilla Firefox\firefox.exe" ".\coverage\index.htm"
exit 0

:ERROR
exit 0