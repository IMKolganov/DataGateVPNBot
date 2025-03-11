# Debug: Show working directory
RUN pwd && ls -la

# Debug: Show contents of DataGateVPNBot/
RUN ls -la DataGateVPNBot || true

# Define architecture as a build argument
ARG TARGETARCH

# Use the .NET SDK image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set the working directory
WORKDIR /src

# Copy the project file and restore dependencies (FIXED PATH)
COPY DataGateVPNBot/DataGateVPNBot.csproj DataGateVPNBot/

WORKDIR /src/DataGateVPNBot
RUN dotnet restore "DataGateVPNBot.csproj"

# Copy the rest of the application source code
COPY DataGateVPNBot/ DataGateVPNBot/

# Build the application
ARG BUILD_CONFIGURATION=Release
WORKDIR /src/DataGateVPNBot
RUN echo "Building for TARGETARCH=${TARGETARCH}" && \
    dotnet build "DataGateVPNBot.csproj" -c ${BUILD_CONFIGURATION} -o /app/build --runtime linux-${TARGETARCH} --self-contained false

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
WORKDIR /src/DataGateVPNBot
RUN echo "Publishing for TARGETARCH=${TARGETARCH}" && \
    dotnet publish "DataGateVPNBot.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish --runtime linux-${TARGETARCH} --self-contained false

# Use the final ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Install curl (optional, useful for debugging)
RUN apt-get update && apt-get install -y curl

# Create a non-root user for security
RUN useradd -m app && mkdir -p /app && chown -R app:app /app

# Switch to the non-root user
USER app

# Set the working directory
WORKDIR /app

# Copy the published application from the previous stage
COPY --from=publish /app/publish .

# Copy configuration files if they exist in the context
COPY appsettings.json . || true
COPY appsettings.Development.json . || true

# Set the entry point
ENTRYPOINT ["dotnet", "DataGateVPNBot.dll"]
