FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

ENV ASPNETCORE_ENVIRONMENT=Development

# Use the official .NET SDK image for the build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project files and restore dependencies
COPY ["DigitalWallet.API/DigitalWallet.API.csproj", "DigitalWallet.API/"]
COPY ["DigitalWallet.Application/DigitalWallet.Application.csproj", "DigitalWallet.Application/"]
COPY ["DigitalWallet.Domain/DigitalWallet.Domain.csproj", "DigitalWallet.Domain/"]
COPY ["DigitalWallet.CrossCutting/DigitalWallet.CrossCutting.csproj", "DigitalWallet.CrossCutting/"]
COPY ["DigitalWallet.Infrastructure/DigitalWallet.Infrastructure.csproj", "DigitalWallet.Infrastructure/"]
RUN dotnet restore "DigitalWallet.API/DigitalWallet.API.csproj"

# Copy the entire source code and build the application
COPY . .
WORKDIR "/src/DigitalWallet.API"
RUN dotnet build "DigitalWallet.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DigitalWallet.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: Use the runtime image to run the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DigitalWallet.API.dll"]
