﻿# Use the ARM64 SDK image for building the app
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the project file and restore dependencies
COPY ["DataGateVPNBotV1.csproj", "./"]
RUN dotnet restore "DataGateVPNBotV1.csproj"

# Copy the rest of the application source code
COPY . .

# Build the application
ARG BUILD_CONFIGURATION=Release
RUN dotnet build "DataGateVPNBotV1.csproj" -c $BUILD_CONFIGURATION -o /app/build --runtime linux-arm64 --self-contained false

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN echo "Using build configuration: $BUILD_CONFIGURATION" && \
    dotnet publish "DataGateVPNBotV1.csproj" -c $BUILD_CONFIGURATION -o /app/publish --runtime linux-arm64 --self-contained false


# Use the ASP.NET runtime for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Install curl (optional, if needed for debugging or HTTP requests)
RUN apt-get update && apt-get install -y curl

# Set up a non-root user for security
RUN if ! id -u app > /dev/null 2>&1; then useradd -m app; fi
RUN mkdir -p /app && chown -R app:app /app

# Switch to the non-root user
USER app

# Set the working directory
WORKDIR /app

# Copy the published application from the build stage
COPY --from=publish /app/publish .

# Copy configuration files (if they exist in the context)
COPY appsettings.json .
COPY appsettings.Development.json .

# Specify the entry point for the application
ENTRYPOINT ["dotnet", "DataGateVPNBotV1.dll"]
