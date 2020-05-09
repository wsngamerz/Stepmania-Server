![.NET Core](https://github.com/wsngamerz/Stepmania-Server/workflows/.NET%20Core/badge.svg)
![GitHub repo size](https://img.shields.io/github/repo-size/wsngamerz/Stepmania-Server)

# Stepmania Server: A Stepmania Server

[Stepmania](https://github.com/stepmania/stepmania)

The plan for this project is to have a fully functioning stepmania server with an intergrated web client which can display live games and statistics to all players and all songs played.

[Stepmania Server Web Client](https://github.com/wsngamerz/Stepmania-Server-Web)

The server at the moment aims to be compatible with all official builds of stepmania.

**Compatible Stepmania Versions:**
[StepMania 5.0.12](https://github.com/stepmania/stepmania/releases/tag/v5.0.12),
[StepMania 5.1-beta](https://github.com/stepmania/stepmania/releases/tag/v5.1.0-b2)

Unfortunatly the Alpha builds of SM5.3 are not compatible yet but when I can get my hands on the updated protocol documentation, the server will try to update to this.

Furthermore, older versions of stepmania and forks of stepmania may or may not work, they are untested.

The storage options will be initially either SQLite or MySQL but I aim to make this server as configurable as possible so more can be added if need be in the future.

## Running

If an installer or a pre-built binary hasn't been distributed, see Building below.

To run stepmania server, all you need installed is the [Dotnet Core Runtime 3.1+](https://dotnet.microsoft.com/download).

Note: if you have previously installed the Dotnet Core SDK, the Dotnet Core Runtime is included with the installation so there is no need to install it again.

## Building

My main development PC runs Windows so I will primarily focus on that but Dotnet Core supports Linux and MacOS as well so if you have the technical knowhow, you should be able to build on those platforms too as nothing in the server depends on any platform specific API's.

### Windows

Requirements:

- [Dotnet Core SDK 3.1+](https://dotnet.microsoft.com/download)
- [git](https://git-scm.com/downloads)
- [nsis](https://nsis.sourceforge.io/Download)
- [nodejs](https://nodejs.org/en/download/)
- npm (bundled with nodejs)

To build on windows simply open a command line and type:

```
git clone https://github.com/wsngamerz/Stepmania-Server
cd Stepmania-Server
.\build.bat
```

NOTE: This currently builds x64, I will expand this to build a x86 version at a later date.

### Linux

Coming soon

### MacOS

Coming soon (but will be unable to test this as I do not own any MacOs devices)
