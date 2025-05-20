# Base runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base

# Create user and group named 'app'
RUN addgroup --system app && adduser --system --ingroup app app

# Switch to user 'app'
USER app

WORKDIR /app
EXPOSE 9090
EXPOSE 9091

# SDK image for build
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy project file and restore dependencies
COPY ["MagicVilla_VillaAPI/MagicVilla_VillaAPI.csproj", "MagicVilla_VillaAPI/"]
RUN dotnet restore "MagicVilla_VillaAPI/MagicVilla_VillaAPI.csproj"

# Copy the rest of the source code and build
COPY . .
WORKDIR "/src/MagicVilla_VillaAPI"
RUN dotnet build "MagicVilla_VillaAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "MagicVilla_VillaAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Use non-root 'app' user
USER app

ENTRYPOINT ["dotnet", "MagicVilla_VillaAPI.dll"]
