FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Copy csproj and restore as distinct layers
WORKDIR /app/AlphabetUpdateServer
COPY ./src/AlphabetUpdateServer/*.csproj ./
#RUN dotnet restore --packages=/packages

WORKDIR /app/AlphabetUpdateServerInstaller
COPY ./src/AlphabetUpdateServerInstaller/*.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY ./src/AlphabetUpdateServerInstaller /app/AlphabetUpdateServerInstaller
COPY ./src/AlphabetUpdateServer /app/AlphabetUpdateServer
RUN dotnet publish -c Release -o /out --no-restore

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /out .