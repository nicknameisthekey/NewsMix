export DOTNET_ENVIRONMENT=Development
cd NewsMix.ConsoleRunner
dotnet publish
cd bin/Debug/net7.0/publish
dotnet NewsMix.ConsoleRunner.dll