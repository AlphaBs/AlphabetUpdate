docker build -t ksi123456ab/alphabet-update-server .
cd docker_bundle
./create_installer.sh
echo "make sure that you run the command 'docker push ksi123456ab/alphabet-update-server' before update!"