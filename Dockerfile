FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Copy csproj and restore as distinct layers
COPY ./src/AlphabetUpdate.Common/*.csproj /src/AlphabetUpdate.Common/
COPY ./src/AlphabetUpdateServerInstaller/*.csproj /src/AlphabetUpdateServerInstaller/
COPY ./src/AlphabetUpdateServer/*.csproj /src/AlphabetUpdateServer/
RUN dotnet restore /src/AlphabetUpdateServerInstaller/AlphabetUpdateServerInstaller.csproj

COPY ./src/AlphabetUpdate.Common /src/AlphabetUpdate.Common
COPY ./src/AlphabetUpdateServerInstaller /src/AlphabetUpdateServerInstaller
COPY ./src/AlphabetUpdateServer /src/AlphabetUpdateServer

WORKDIR /src/AlphabetUpdateServerInstaller
RUN dotnet publish -c Release -o /out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /out .
COPY ./start_update_server.sh /app/start_update_server.sh
CMD ["/bin/bash", "/app/start_update_server.sh"]
