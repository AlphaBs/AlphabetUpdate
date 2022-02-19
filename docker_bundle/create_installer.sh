#! /bin/bash

echo Start create_installer

mkdir ./app
cp .env ./app
cp ./app.sh ./app
cp ./docker-compose.yml ./app
mkdir -p ./app/volume/in
mkdir -p ./app/volume/out
mkdir -p ./app/volume/vnstat
mkdir -p ./app/volume/nginx
cp ./volume/nginx.conf ./app/volume
cp ./volume/appsettings.json ./app/volume
cp -r ./volume/nginx ./app/volume

tar -czf app.tar.gz ./app/*
rm -r ./app
echo Done create_installer