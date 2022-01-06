if [[ ${START_OPTION} == "INSTALL" ]]; then
    if [[ ${DEBUG} == "TRUE" ]]; then
        dotnet AlphabetUpdateServerInstaller.dll --debug
    else
        dotnet AlphabetUpdateServerInstaller.dll
    fi
elif [[ ${START_OPTION} == "SERVER" ]]; then
    dotnet AlphabetUpdateServer.dll
else
    echo "Unrecognizable start option: ${START_OPTION}"
fi