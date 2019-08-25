@echo off

if not exist "..\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" goto error_console
if not exist "..\Test-Easly-Number\bin\x64\Debug\Test-Easly-Number.dll" goto error_EaslyNumber
del *.log
"..\packages\NUnit.ConsoleRunner.3.9.0\tools\nunit3-console.exe" --trace=Debug --labels=All "./bin/x64/Debug/Test-Easly-Number.dll"
goto end

:error_console
echo ERROR: nunit3-console not found.
goto end

:error_EaslyNumber
echo ERROR: Test-Easly-Number.dll not built.
goto end

:end
