@echo off

if not exist "..\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" goto error_console
if not exist "..\Test-FormattedNumber\bin\x64\Release\Test-FormattedNumber.dll" goto error_FormattedNumber
"..\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" --trace=Debug --labels=All "..\Test-FormattedNumber\bin\x64\Release\Test-FormattedNumber.dll"
goto end

:error_console
echo ERROR: nunit3-console not found.
goto end

:error_FormattedNumber
echo ERROR: Test-FormattedNumber.dll not built.
goto end

:end
