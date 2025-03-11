# Define architecture as a build argument
ARG TARGETARCH=amd64

# Use .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Debug: Show working directory and contents
RUN pwd && ls -la

# Set working directory
WORKDIR /src

# Copy solution file
COPY DataGateVPNBot.sln ./

# Copy all project files before restore
COPY DataGateVPNBot/*.csproj DataGateVPNBot/
COPY DataGateVPNBot.DataBase/*.csproj DataGateVPNBot.DataBase/
COPY DataGateVPNBot.Models/*.csproj DataGateVPNBot.Models/

# Restore dependencies (optimize caching)
RUN dotnet restore DataGateVPNBot.sln

# Copy full source code after restore
COPY . .

# Build the solution (not just .csproj)
ARG BUILD_CONFIGURATION=Release
RUN echo "Building for TARGETARCH=${TARGETARCH}" && \
    dotnet build DataGateVPNBot.sln -c ${BUILD_CONFIGURATION} -o /app/build

# Publish the application
FROM build AS publish
RUN echo "Publishing for TARGETARCH=${TARGETARCH}" && \
    dotnet publish DataGateVPNBot.sln -c ${BUILD_CONFIGURATION} -o /app/publish --runtime linux-${TARGETARCH:-amd64} --self-contained false

# Use the final ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Install curl (optional, useful for debugging)
RUN apt-get update && apt-get install -y curl

# Ensure the non-root user exists
RUN id -u app >/dev/null 2>&1 || useradd -m app

# Ensure the working directory exists and has correct permissions
RUN mkdir -p /app && chown -R app:app /app

# Switch to the non-root user
USER app

# Set working directory
WORKDIR /app

# Copy published application from the build stage
COPY --from=publish /app/publish .

# Copy configuration files if they exist
COPY appsettings.json .
COPY appsettings.Development.json .

# Set the entry point
ENTRYPOINT ["dotnet", "DataGateVPNBot.dll"]