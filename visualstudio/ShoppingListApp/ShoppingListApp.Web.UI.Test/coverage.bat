@ECHO OFF
"..\..\..\packages\OpenCover.4.5.3723\OpenCover.Console.exe" -target:"..\..\..\packages\NUnit.Runners.2.6.4\tools\nunit-console.exe" -targetargs:"ShoppingListApp.Web.UI.Test.dll /noshadow /nologo /framework:net-4.5.1" -register:"user" -mergebyhash -mergeoutput -output:"..\..\..\coverage\ShoppingListApp.Web.UI.Test.xml"
if %ERRORLEVEL% GEQ 1 GOTO ERROR

echo starting reportgenerator
"..\..\..\packages\ReportGenerator.2.1.3.0\reportgenerator.exe" -reports:"..\..\..\coverage\ShoppingListApp.Domain.Test.xml;..\..\..\coverage\ShoppingListApp.I18N.Utils.Test.xml;..\..\..\coverage\ShoppingListApp.JobsScheduler.Test.xml;..\..\..\coverage\ShoppingListApp.Web.Test.xml;..\..\..\coverage\ShoppingListApp.Web.UI.Test.xml" -targetdir:"..\..\..\coverage" Verbosity:"Error"
if %ERRORLEVEL% GEQ 1 GOTO ERROR

REM Firefox is causing the batch file to return an error code 1
"C:\Program Files (x86)\Mozilla Firefox\firefox.exe" "..\..\..\coverage\index.htm"
exit 0

:ERROR
exit 0