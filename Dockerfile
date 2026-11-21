
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build

COPY ["Devices/Devices.API/Devices.API.csproj", "Devices/Devices.API/"]
COPY ["Devices/Devices.Models/Devices.Models.csproj", "Devices/Devices.Models/"]
RUN dotnet restore "Devices/Devices.Models/Devices.Models.csproj"
RUN dotnet restore "Devices/Devices.API/Devices.API.csproj"

COPY . .
WORKDIR Devices/Devices.API

RUN dotnet publish "Devices.API.csproj" \
    -c Release \
    -o /app/publish \
    --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final

WORKDIR /app

RUN ls -la

COPY --from=build /app/publish .

RUN chmod 777 /app
RUN chmod 777 devices.db

ENV ASPNETCORE_HTTP_PORTS=8080

EXPOSE 8080

USER $APP_UID

ENTRYPOINT ["dotnet", "Devices.API.dll"]
