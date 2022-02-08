#! /bin/bash

if [[ $1 == "install" ]]; then
    docker-compose run asp-install
elif [[ $1 == "start" ]]; then
    docker-compose up -d
elif [[ $1 == "stop" ]]; then
    docker-compose stop
elif [[ $1 == "down" ]]; then
    docker-compose down
elif [[ $1 == "update" ]]; then
    docker pull ksi123456ab/alphabet-update-server
    docker-compose restart
else
    echo "No matching command"
fi