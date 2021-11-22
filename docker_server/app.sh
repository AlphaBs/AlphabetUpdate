#! /bin/bash

if [[ $1 == "install" ]]; then
    docker-compose run asp-install
elif [[ $1 == "start" ]]; then
    docker-compose up -d
elif [[ $1 == "stop" ]]; then
    docker-compose stop
elif [[ $1 == "down" ]]; then
    docker-compose down
elif [[ $1 == "build" ]]; then
    docker build -t ksi123456ab/alphabet-update-server .
    ./create_installer.sh
else
    echo "No matching command"
fi