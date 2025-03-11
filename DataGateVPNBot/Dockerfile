# Define architecture before the build
ARG TARGETARCH=amd64

# Use .NET SDK for building
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Set environment variable for persistence
ENV TARGETARCH=${TARGETARCH}

# Set the working directory
WORKDIR /src

# Copy project file and restore dependencies
COPY ["DataGateVPNBotV1.csproj", "./"]
RUN dotnet restore "DataGateVPNBotV1.csproj"

# Copy the rest of the application source code
WORKDIR /src
COPY . .

# Build the application for the correct architecture
ARG BUILD_CONFIGURATION=Release
RUN echo "Building for TARGETARCH=${TARGETARCH}" && \
    dotnet build "DataGateVPNBotV1.csproj" -c ${BUILD_CONFIGURATION} -o /app/build --runtime linux-${TARGETARCH} --self-contained false

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
ARG TARGETARCH
# Ensure TARGETARCH is passed again
RUN echo "Publishing for TARGETARCH=${TARGETARCH}" && \
    dotnet publish "DataGateVPNBotV1.csproj" -c ${BUILD_CONFIGURATION} -o /app/publish --runtime linux-${TARGETARCH} --self-contained false

# Use the final ASP.NET runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Install curl (if needed for debugging or HTTP requests)
RUN apt-get update && apt-get install -y curl

# Create a non-root user for security
RUN if ! id -u app > /dev/null 2>&1; then useradd -m app; fi
RUN mkdir -p /app && chown -R app:app /app

# Switch to the non-root user
USER app

# Set the working directory
WORKDIR /app

# Copy the published application from the previous stage
COPY --from=publish /app/publish .

# Copy configuration files (if they exist in the context)
COPY appsettings.json .
COPY appsettings.Development.json .

# Set the entry point for the application
ENTRYPOINT ["dotnet", "DataGateVPNBotV1.dll"]