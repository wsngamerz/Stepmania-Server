@echo off
@setlocal

REM Build start time
set start=%time%

echo Building stepmania server for Windows x64
echo =========================================

echo Checking dependencies
set /A missing_deps = 0

REM TODO: Make recursive

where dotnet >nul 2>nul
if %ERRORLEVEL% == 0 (
    echo + dotnet found
) else (
    echo - dotnet not found
    set missing_deps = %missing_deps% + 1
)

where node >nul 2>nul
if %ERRORLEVEL% == 0 (
    echo + node found
) else (
    echo - node not found
    set missing_deps = %missing_deps% + 1
)

where npm >nul 2>nul
if %ERRORLEVEL% == 0 (
    echo + npm found
) else (
    echo - npm not found
    set missing_deps = %missing_deps% + 1
)

where git >nul 2>nul
if %ERRORLEVEL% == 0 (
    echo + git found
) else (
    echo - git not found
    set missing_deps = %missing_deps% + 1
)

where makensis >nul 2>nul
if %ERRORLEVEL% == 0 (
    echo + makensis found
) else (
    echo - makensis not found
    set missing_deps = %missing_deps% + 1
)

if %missing_deps% gtr 0 (
    echo missing %missing_deps% dependencies
    exit /b 1
)


REM clean up after previous builds
if exist "bin\" (
    echo Removing existing bin directory
    rmdir /S /q "bin\"
)

if exist "build\" (
    echo Removing existing build directory
    rmdir /S /q "build\"
)

if exist "dist\" (
    echo Removing existing dist directory
    rmdir /S /q "dist\"
)

if exist "obj\" (
    echo Removing existing obj directory
    rmdir /S /q "obj\"
)

REM Start building
echo Building windows x64 binaries
dotnet publish --runtime win-x64 --configuration Release --no-self-contained -o "build\win-x64" --nologo --verbosity minimal

echo Cloning web client
git clone https://github.com/wsngamerz/Stepmania-Server-Web ./build/web

echo Installing web dependencies
cd ./build/web
call npm install --loglevel warn

echo Building web client
call npm run build

echo Copying web client into final build
if not exist "..\win-x64\Web" mkdir "..\win-x64\Web"
xcopy build ..\win-x64\Web\ /E/H/Y
cd ..\..\

echo Moving built files to dist
if not exist ".\dist\win-x64" mkdir ".\dist\win-x64"
xcopy .\build\win-x64 .\dist\win-x64\ /E/H/Y

echo Creating Installer
call makensis .\installer.nsi

echo Complete

REM Calculate time taken to run full build
set end=%time%
set options="tokens=1-4 delims=:.,"
for /f %options% %%a in ("%start%") do set start_h=%%a&set /a start_m=100%%b %% 100&set /a start_s=100%%c %% 100&set /a start_ms=100%%d %% 100
for /f %options% %%a in ("%end%") do set end_h=%%a&set /a end_m=100%%b %% 100&set /a end_s=100%%c %% 100&set /a end_ms=100%%d %% 100

set /a hours=%end_h%-%start_h%
set /a mins=%end_m%-%start_m%
set /a secs=%end_s%-%start_s%
set /a ms=%end_ms%-%start_ms%
if %ms% lss 0 set /a secs = %secs% - 1 & set /a ms = 100%ms%
if %secs% lss 0 set /a mins = %mins% - 1 & set /a secs = 60%secs%
if %mins% lss 0 set /a hours = %hours% - 1 & set /a mins = 60%mins%
if %hours% lss 0 set /a hours = 24%hours%
if 1%ms% lss 100 set ms=0%ms%

set /a totalsecs = %hours%*3600 + %mins%*60 + %secs%
echo command took %hours%:%mins%:%secs%.%ms% (%totalsecs%.%ms%s total)
