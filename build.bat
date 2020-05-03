Rem Build StepmaniaServer

Rem This should package in one file but a bug with this is causing errors with including some external
Rem Packages so unable to package in 1 file.
Rem dotnet publish -r win-x64 -c Release /p:PublishTrimmed=true /p:PublishSingleFile=true

dotnet publish -r win-x64 -c Release /p:PublishTrimmed=true
