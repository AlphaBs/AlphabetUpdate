#! /bin/bash

mkdir ./app
cp .env ./app
cp ./app.sh ./app
cp ./docker-compose.yml ./app
mkdir -p ./app/volume/in
mkdir -p ./app/volume/out
cp ./volume/nginx.conf ./app/volume
cp ./volume/appsettings.json ./app/volume

tar czf app.tar.gz ./app/*
rm -r ./app
echo Done.