# squaresApi

## About
API that from a given set of points in a 2D plane - enables the consumer to find out sets of points that make squares and how many squares can be drawn. A point is a pair of integer X and Y coordinates. A square is a set of 4 points that when connected make up, well, a square.

- Example of a list of points that make up a square: [(-1;1), (1;1), (1;-1), (-1;-1)]

## Tech
- [.NET Core](https://www.microsoft.com/net/core/platform)
- [Entity Framework Core](https://docs.microsoft.com/en-us/ef/core/)
- [Microsoft/dotnet docker image](https://hub.docker.com/r/microsoft/dotnet/)


## Use cases
- I as a user can import a list of points
- I as a user can add a point to an existing list
- I as a user can delete a point from an existing list
- I as a user can retrieve the squares identified

## Getting Started

### Prerequisities

Working and accessible SQL database

### Run a docker container:
1. Clone or download this repository to local machine.
2. Install 
* [Windows](https://docs.docker.com/windows/started)
* [OS X](https://docs.docker.com/mac/started/)
* [Linux](https://docs.docker.com/linux/started/) if didn't install yet.
3. Set up database default connection string in `SquaresAPI/appsettings.json`
4. `sudo docker build -t enter-name-your-docker-container .`
5. `sudo docker run -p 5000:5000 -it enter-name-your-docker-container` or `sudo docker run -p 5000:5000 -d enter-name-your-docker-container` to run detached.

### Run local with CLI
1. Clone or download this repository to local machine.
2. Install [.NET Core SDK for your platform](https://www.microsoft.com/net/core#windowscmd) if didn't install yet.
3. Set up database default connection string in `SquaresAPI/appsettings.json`
4. `dotnet restore`
5. `dotnet run`

### Run on Visual Studio
1. Install [Visual Studio 2019 for your platform](https://www.visualstudio.com/vs/) if didn't install yet.
2. Open project
3. Debug -> Start debugging

