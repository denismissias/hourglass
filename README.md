# Hourglass

Hourglass is a time-tracking API built with ASP.NET Core Minimal API, JWT authentication, MySQL, and native OpenAPI.

## Technologies and Tools

- .NET 10
- ASP.NET Core Minimal API
- Native OpenAPI
- Scalar API Reference
- JWT Bearer authentication
- Entity Framework Core with Pomelo MySQL provider
- MySQL 8.0
- Docker and Docker Compose
- xUnit, FluentAssertions, Moq, and EF Core InMemory

## Current API Surface

The API currently exposes these endpoints:

- `POST /api/v{version}/authenticate`
- `POST /api/v{version}/users`
- `PUT /api/v{version}/users/{id}`
- `GET /api/v{version}/users/{id}`
- `POST /api/v{version}/times`
- `GET /health`

Routes under `/users` and `/times` require a valid JWT bearer token.

## Running with Docker

1. Go to the `Hourglass` folder.
2. Run `docker-compose up -d`.

The API is exposed at `http://localhost:3000`.

API documentation:

- Scalar UI: `http://localhost:3000/scalar/v1`
- OpenAPI document: `http://localhost:3000/openapi/v1.json`

## Running locally

1. Go to the `Hourglass/Hourglass` folder.
2. Run `dotnet run`.

Development profiles are configured to open the API documentation automatically.

Default local URLs:

- HTTP: `http://localhost:5212`
- HTTPS: `https://localhost:7173`

Local API documentation:

- Scalar UI: `/scalar/v1`
- OpenAPI document: `/openapi/v1.json`

## Authentication

Authentication is done with JWT Bearer tokens.

1. Call `POST /api/v1/authenticate` with login and password.
2. Copy the returned token.
3. Send the token in the `Authorization` header using `Bearer {token}`.

JWT settings are configured through `appsettings.json`, `appsettings.Development.json`, or User Secrets.

### Example: Authenticate

Request:

```json
{
	"login": "admin",
	"password": "admin"
}
```

Response:

```json
{
	"token": "eyJhbGciOi...",
	"user": {
		"userId": 1,
		"login": "admin",
		"name": "Admin",
		"email": "admin@admin.com"
	}
}
```

## Request and Response Examples

### Example: Create User

Request:

```json
{
	"name": "Jane Doe",
	"email": "jane.doe@example.com",
	"login": "jane.doe",
	"password": "StrongPassword123"
}
```

Response:

```json
{
	"userId": 2,
	"login": "jane.doe",
	"name": "Jane Doe",
	"email": "jane.doe@example.com"
}
```

### Example: Create Time Entry

Request:

```json
{
	"project_id": 1,
	"user_id": 1,
	"started_at": "2026-04-25T08:00:00Z",
	"ended_at": "2026-04-25T12:00:00Z"
}
```

Response:

```json
{
	"time_id": 1,
	"project_id": 1,
	"user_id": 1,
	"started_at": "2026-04-25T08:00:00Z",
	"ended_at": "2026-04-25T12:00:00Z"
}
```

## Seed Data

On a clean database initialization through Docker, the SQL bootstrap script creates:

- one default user
- login: `admin`
- password: `admin`
- 10 sample projects with IDs from `1` to `10`

If the MySQL volume already exists, this data may already be present from a previous run.

## Running Tests

Run all automated tests with:

`dotnet test`

The solution includes unit tests for repositories and token generation.

## Notes

- OpenAPI and Scalar are mapped in Development.
- The health endpoint is available at `/health`.
