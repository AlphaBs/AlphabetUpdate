#! /bin/bash

mkdir ./installer
cp ./app.sh ./installer
cp ./docker-compose.yml ./installer
mkdir -p ./installer/volume/in
mkdir -p ./installer/volume/out
cp ./volume/nginx.conf ./installer/volume

tar czf installer.tar.gz ./installer
rm -r ./installer
echo Done.