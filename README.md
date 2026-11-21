# Confidential-Devices-API
Coding challenge regarding a device's API from a confidential company.

## Overview

An ASP.NET Core API for creating, querying, updating, and deleting devices. It uses .NET 9, Entity Framework Core with SQLite, and FluentValidation. The solution also includes integration tests.

## Projects

- `Devices/Devices.API` — API endpoints and application configuration.
- `Devices/Devices.Models` — device models, database context, and validation rules.
- `Devices/Devices.Tests` — API integration tests.

## Getting started

Install the [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0), then run the following from the repository root:

```powershell
dotnet restore Devices/Devices.slnx
dotnet build Devices/Devices.slnx
dotnet run --project Devices/Devices.API --launch-profile http
```

The HTTP launch profile listens at `http://localhost:5260`. The `https` profile also provides `https://localhost:7230`.

Explore the API using the Scalar interface at `/scalar` or the OpenAPI document at `/openapi/v1.json`.

### Database

The API uses the included SQLite database, `Devices/Devices.API/devices.db`. Its connection string is `Data Source=devices.db`, so the database path is relative to the application's working directory. Keep a backup if you need to preserve local data.

### Docker

With Docker and Docker Compose installed, run from the repository root:

```powershell
docker compose up --build
```

The API is available at `http://localhost:8080`. Stop it with `docker compose down`. The Compose configuration does not mount a volume for the database, so changes inside the container are not preserved when the container is removed.

## Running tests

From the repository root:

```powershell
dotnet test Devices/Devices.slnx
```

## Device API

The base route is `/api/devices`.

| Method | Route | Description |
| --- | --- | --- |
| POST | `/api/devices/create` | Create a device. |
| GET | `/api/devices` | List all devices. |
| GET | `/api/devices/{id}` | Get a device by ID. |
| GET | `/api/devices/brand?brand=Google` | List devices by brand. |
| GET | `/api/devices/state?state=Available` | List devices by state. |
| PUT | `/api/devices/update` | Update supplied fields of a device. |
| DELETE | `/api/devices/{id}` | Delete a device. |

Devices have an `id`, `name`, `brand`, `state`, and UTC `creationTime`. States are `Available`, `InUse`, and `Inactive`; a new device defaults to `Available` if its state is omitted.

### Example requests

Create a device:

```http
POST /api/devices/create
Content-Type: application/json

{
  "name": "Pixel 9",
  "brand": "Google",
  "state": "Available"
}
```

Update its state, using the ID returned by the create request:

```http
PUT /api/devices/update
Content-Type: application/json

{
  "id": 1,
  "state": "InUse"
}
```

### Validation and behavior

- On creation, name and brand are required and limited to 64 characters.
- Creation time is set by the server and is not included in the update payload.
- A device in the `InUse` state cannot be deleted or have its name or brand changed; its state can still be updated.
- Validation failures return `400 Bad Request`. Looking up, updating, or deleting a nonexistent device also returns `400`.
- Brand filtering uses SQLite's `LIKE` behavior, including support for `%` and `_` wildcards.
