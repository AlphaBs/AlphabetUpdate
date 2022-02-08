#! /bin/bash

echo Start create_installer

mkdir ./app
cp .env ./app
cp ./app.sh ./app
cp ./docker-compose.yml ./app
mkdir -p ./app/volume/in
mkdir -p ./app/volume/out
cp ./nginx.conf ./app/volume
cp ./appsettings.json ./app/volume

tar -czf app.tar.gz ./app/*
rm -r ./app
echo Done create_installer