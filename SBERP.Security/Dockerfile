# See https://aka.ms/customizecontainer to learn how to customize your debug container
# and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# =========================
# Base (runtime)
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app

# Make the app listen on both HTTP and HTTPS inside the container
# VS uses these too. Safe defaults for CLI.
ENV ASPNETCORE_HTTP_PORTS=8082
ENV ASPNETCORE_HTTPS_PORTS=8083

EXPOSE 8082
EXPOSE 8083

# NOTE:
# Visual Studio sometimes adds this line for non-root debugging.
# If you run manually and APP_UID isn't set, it can cause issues.
# Uncomment only if you know APP_UID is configured for your runtime.
# USER $APP_UID  # <-- keep commented unless you know you need it


# =========================
# Build
# =========================
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# copy csproj files first (better build cache)
COPY ["SBERP.Security/SBERP.Security.csproj", "SBERP.Security/"]
COPY ["SBERP.DataAccessLayer/SBERP.DataAccessLayer.csproj", "SBERP.DataAccessLayer/"]
COPY ["SBERP.EmailService/SBERP.EmailService.csproj", "SBERP.EmailService/"]

# restore
RUN dotnet restore "SBERP.Security/SBERP.Security.csproj"

# copy the rest
COPY . .

# build
WORKDIR "/src/SBERP.Security"
RUN dotnet build "SBERP.Security.csproj" -c $BUILD_CONFIGURATION -o /app/build


# =========================
# Publish
# =========================
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SBERP.Security.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false


# =========================
# Final
# =========================
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SBERP.Security.dll"]

# ===========Image & Container Commands==============
# Create Image
# docker rmi sberp-security:latest (If you want to remove old image)
# docker builder prune -a -f (To clean up all unused images)
# cd "D:\Personal Work\Project\SB.Web\SBERP.Services"
# docker build -t sberp-security -f SBERP.Security\Dockerfile .
# docker images

# Run Container
# docker rm contSBERPSecurity --force (If you want to remove old container)
# docker ps --all (To check all containers)
# docker run -d --rm --name contSBERPSecurity `-p 51939:8082 -p 44357:8083 `-e ASPNETCORE_Kestrel__Certificates__Default__Path=/https/SBERP.Security.pfx `-e ASPNETCORE_Kestrel__Certificates__Default__Password="sberp" `-v "$env:USERPROFILE\.aspnet\https:/https:ro" `sberp-security
# docker ps --all
# =========================