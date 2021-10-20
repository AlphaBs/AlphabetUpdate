#! /bin/bash

if [[ $1 == "install" ]]; then
    docker-compose up -d
    docker exec -it $(docker-compose ps -q asp) dotnet AlphabetUpdateServerInstaller.dll --debug
    docker exec -d $(docker-compose ps -q asp) dotnet AlphabetUpdateServer.dll
elif [[ $1 == "start" ]]; then
    docker-compose up -d
    docker exec -d $(docker-compose ps -q asp) dotnet AlphabetUpdateServer.dll
elif [[ $1 == "stop" ]]; then
    docker stop $(docker-compose ps -q)
elif [[ $1 == "clean" ]]; then
    docker rm -f $(docker-compose ps -q)
else
    echo "No matching command"
fi