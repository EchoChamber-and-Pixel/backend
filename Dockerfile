FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /app

# https://github.com/NuGet/Announcements/issues/49
RUN apt update && apt upgrade -y

COPY *.sln .
COPY EchoChamber.API/*.csproj ./EchoChamber.API/
RUN dotnet restore

COPY EchoChamber.API/. ./EchoChamber.API/
WORKDIR /app/EchoChamber.API
RUN dotnet publish -c release -o ../out

FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build /app/out ./
ENTRYPOINT ["dotnet", "EchoChamber.API.dll"]