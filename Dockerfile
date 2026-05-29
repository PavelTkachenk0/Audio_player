# syntax=docker/dockerfile:1

# ---- Build stage ----
# Pinned to the .NET 8 SDK on purpose (the app targets net8.0), even though a
# newer SDK may be installed locally.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the project files first so dotnet restore can be cached as a layer.
# The folder layout must match the relative ProjectReference
# (Audio_player.csproj references ..\Audio_player.DAL\Audio_player.DAL.csproj).
COPY Audio_player/Audio_player.csproj Audio_player/
COPY Audio_player.DAL/Audio_player.DAL.csproj Audio_player.DAL/
RUN dotnet restore Audio_player/Audio_player.csproj

# Copy the remaining source and publish a self-contained framework-dependent app.
COPY . .
RUN dotnet publish Audio_player/Audio_player.csproj -c Release -o /app/publish

# ---- Runtime stage ----
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "Audio_player.dll"]
