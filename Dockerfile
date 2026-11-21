
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

WORKDIR /src

COPY ["Devices/Devices.API/Devices.API.csproj", "src/Devices.API/"]
RUN dotnet restore "src/Devices.API/Devices.API.csproj"

COPY . .
WORKDIR /src/src/Devices.API

RUN dotnet publish "Devices.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

COPY --from=build /app/publish .

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "Devices.API.dll"]
