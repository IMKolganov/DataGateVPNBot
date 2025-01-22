﻿# Use the SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Install dependencies and build the project
WORKDIR /src
COPY ["DataGateVPNBotV1.csproj", "."]
RUN dotnet restore "DataGateVPNBotV1.csproj"
COPY . .
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "DataGateVPNBotV1.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "DataGateVPNBotV1.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final layer with the app user
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
RUN apt-get update && apt-get install -y curl

# Check and create the app user if it does not exist
RUN if ! id -u app > /dev/null 2>&1; then useradd -m app; fi
RUN mkdir -p /app && chown -R app:app /app

USER app
WORKDIR /app
COPY --from=publish /app/publish .

# Copy configuration files
COPY appsettings.json .
COPY appsettings.Development.json .

# Specify the command to run
ENTRYPOINT ["dotnet", "DataGateVPNBotV1.dll"]
