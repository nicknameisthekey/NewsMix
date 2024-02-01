export DOTNET_ENVIRONMENT=Development
export GITCOMMITURL=https://github.com/nicknameisthekey/NewsMix/commit/$(git rev-parse HEAD)
cd NewsMix.ConsoleRunner
dotnet publish
cd bin/Debug/net7.0/publish
dotnet NewsMix.ConsoleRunner.dll 