# ---------- build ----------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy only the project files first: the (slow) restore layer is cached until a package changes.
COPY AppuntaMed.API/AppuntaMed.API.csproj AppuntaMed.API/
COPY AppuntaMed.Application/AppuntaMed.Application.csproj AppuntaMed.Application/
COPY AppuntaMed.Domain/AppuntaMed.Domain.csproj AppuntaMed.Domain/
COPY AppuntaMed.Notifications/AppuntaMed.Notifications.csproj AppuntaMed.Notifications/
COPY AppuntaMed.Persistence/AppuntaMed.Persistence.csproj AppuntaMed.Persistence/
COPY AppuntaMed.Security/AppuntaMed.Security.csproj AppuntaMed.Security/
RUN dotnet restore AppuntaMed.API/AppuntaMed.API.csproj

COPY . .
RUN dotnet publish AppuntaMed.API/AppuntaMed.API.csproj -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---------- runtime ----------
# Keep a Debian-based image: it ships tzdata and ICU, which the reminder job needs for its time zone.
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

# curl is only used by the container HEALTHCHECK.
RUN apt-get update \
    && apt-get install -y --no-install-recommends curl \
    && rm -rf /var/lib/apt/lists/*

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080
EXPOSE 8080

HEALTHCHECK --interval=15s --timeout=5s --start-period=30s --retries=5 \
    CMD curl -fsS http://localhost:8080/health || exit 1

# Do not run as root ($APP_UID is defined by the base image).
USER $APP_UID
ENTRYPOINT ["dotnet", "AppuntaMed.API.dll"]
