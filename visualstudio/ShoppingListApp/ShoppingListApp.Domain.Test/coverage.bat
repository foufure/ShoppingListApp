@ECHO OFF
"..\..\..\packages\OpenCover.4.5.3723\OpenCover.Console.exe" -target:"..\..\..\packages\NUnit.Runners.2.6.4\tools\nunit-console.exe" -targetargs:"ShoppingListApp.Domain.Test.dll /noshadow /nologo /framework:net-4.5.1" -register:"user" -mergebyhash -mergeoutput -output:"..\..\..\coverage\ShoppingListApp.Domain.Test.xml"
if %ERRORLEVEL% GEQ 1 GOTO ERROR

:ERROR
exit 0