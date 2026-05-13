# Hourglass Project Map

This page is a fast reference for AI-assisted changes. It explains where each concern lives and what to check before editing.

## High-Level Structure

- [Hourglass/Hourglass/Program.cs](../../Hourglass/Hourglass/Program.cs) wires services, auth, OpenAPI, CORS, and endpoint registration.
- [Hourglass/Hourglass/Endpoints/](../../Hourglass/Hourglass/Endpoints) contains Minimal API endpoint groups and request/response contracts.
- [Hourglass/Hourglass/Repository/](../../Hourglass/Hourglass/Repository) contains EF Core data access and repository implementations.
- [Hourglass/Hourglass/Services/](../../Hourglass/Hourglass/Services) contains application services such as token generation.
- [Hourglass/Hourglass.Tests/](../../Hourglass/Hourglass.Tests) contains repository and service tests.

## Request Flow

1. `Program.cs` configures dependency injection, authentication, authorization, and OpenAPI.
2. Endpoint extension classes register route groups under `Endpoints/`.
3. Request validation is handled by endpoint filters where required.
4. Endpoints call repository and service abstractions.
5. Repositories persist data through `MySqlContext`.
6. Tests cover repository and service behavior without relying on a live MySQL server.

## Endpoint Areas

### Authentication

- Files: [Hourglass/Hourglass/Endpoints/Authentication/](../../Hourglass/Hourglass/Endpoints/Authentication)
- Purpose: authenticate a user and issue a JWT.
- Important behavior: failed credentials return `ProblemDetails`; successful authentication returns a token and user payload.

### Users

- Files: [Hourglass/Hourglass/Endpoints/Users/](../../Hourglass/Hourglass/Endpoints/Users)
- Purpose: create, update, and fetch user data.
- Important behavior: routes require authorization where appropriate; contract changes should keep DTOs and validation aligned.

### Times

- Files: [Hourglass/Hourglass/Endpoints/Times/](../../Hourglass/Hourglass/Endpoints/Times)
- Purpose: create time entries.
- Important behavior: time range validation, user/project existence checks, and authorization are all part of the route contract.

## Data Layer

- [Hourglass/Hourglass/Repository/MySqlContext.cs](../../Hourglass/Hourglass/Repository/MySqlContext.cs) is the EF Core context.
- Repository interfaces live under [Hourglass/Hourglass/Repository/Interfaces/](../../Hourglass/Hourglass/Repository/Interfaces).
- Concrete repositories live beside the context and typically log, validate inputs, and translate persistence concerns.

## Service Layer

- [Hourglass/Hourglass/Services/TokenService.cs](../../Hourglass/Hourglass/Services/TokenService.cs) generates JWT tokens.
- Token settings come from [Hourglass/Hourglass/Configuration/JwtSettings.cs](../../Hourglass/Hourglass/Configuration/JwtSettings.cs) and appsettings.

## Test Strategy

- Repository tests use EF Core InMemory and mock logging.
- Service tests validate token shape, claims, and expiration.
- When AI changes behavior, add tests near the affected layer first, then update any higher-level documentation.

## Editing Rules For AI

1. Prefer the nearest owning abstraction instead of broad changes.
2. Preserve OpenAPI metadata when touching endpoint files.
3. Keep `ProblemDetails` shape consistent for client and domain errors.
4. Update docs if behavior or routes change.
5. Run `dotnet restore`, `dotnet build`, and `dotnet test` before merging.

## How To Change Each Area

### Authentication Changes

Use [Hourglass/Hourglass/Endpoints/Authentication/AuthenticationEndpoints.cs](../../Hourglass/Hourglass/Endpoints/Authentication/AuthenticationEndpoints.cs) when changing login or token behavior.

- Keep the `ValidationEndpointFilter` on request contracts.
- Preserve the `BadRequest` response for invalid credentials.
- Update [Hourglass/Hourglass/Services/TokenService.cs](../../Hourglass/Hourglass/Services/TokenService.cs) if claim content, issuer, or expiration changes.
- Add or adjust tests in [Hourglass/Hourglass.Tests/Services/](../../Hourglass/Hourglass.Tests/Services) for token behavior.

### Users Changes

Use [Hourglass/Hourglass/Endpoints/Users/UserEndpoints.cs](../../Hourglass/Hourglass/Endpoints/Users/UserEndpoints.cs) for user CRUD behavior.

- Keep `RequireAuthorization()` on the group unless the route is intentionally public.
- Preserve `WithSummary`, `WithDescription`, and `Produces(...)` metadata.
- Hash passwords before persisting users.
- Add or update repository tests in [Hourglass/Hourglass.Tests/Repository/](../../Hourglass/Hourglass.Tests/Repository) when persistence behavior changes.

### Times Changes

Use [Hourglass/Hourglass/Endpoints/Times/TimeEndpoints.cs](../../Hourglass/Hourglass/Endpoints/Times/TimeEndpoints.cs) when changing time entry creation or validation.

- Keep time range validation near the endpoint boundary.
- Keep user/project existence checks aligned with repository interfaces.
- Preserve `Created` responses with the canonical route shape.
- Update repository tests for time persistence rules and service tests only if token/auth behavior is affected.

### Repository Changes

Use the repository implementation closest to the entity you are changing.

- Keep input validation explicit with `ArgumentNullException.ThrowIfNull` or `ThrowIfNullOrEmpty`.
- Use `AsNoTracking()` for read-only queries where appropriate.
- Preserve logging for create/update operations.
- Add tests for new query paths, validation branches, and state changes.

### Cross-Cutting Changes

If a change touches more than one layer, update in this order:

1. Contracts and endpoint behavior.
2. Repository/service logic.
3. Tests for the touched slice.
4. README or onboarding docs if the public behavior changed.

This order keeps AI edits small and makes regressions easier to isolate.

## Prompt Examples

Use these as starting points when asking AI to make changes in this repository.

### Create User

```text
Task:
Add a new endpoint to create users with the same contract style already used in UserEndpoints.

Constraints:
- Keep the Minimal API pattern under Endpoints/Users.
- Preserve authorization on the users group.
- Return ProblemDetails for duplicate or invalid data.
- Add WithSummary, WithDescription, and Produces metadata.
- Add or update tests for the changed behavior.

Verification:
- dotnet restore Hourglass/Hourglass.sln
- dotnet build Hourglass/Hourglass.sln
- dotnet test Hourglass/Hourglass.sln
```

### Change Authentication

```text
Task:
Update authentication so the token payload includes the new claim required by the business rule.

Constraints:
- Keep the authentication endpoint contract explicit.
- Preserve the invalid-credentials ProblemDetails response.
- Update TokenService and its tests together.
- Keep OpenAPI metadata complete on the endpoint.

Verification:
- dotnet restore Hourglass/Hourglass.sln
- dotnet build Hourglass/Hourglass.sln
- dotnet test Hourglass/Hourglass.sln
```

### Create Time Entry

```text
Task:
Adjust the time entry endpoint to support a new validation rule without changing the route shape.

Constraints:
- Keep authorization on the times group.
- Keep validation near the endpoint boundary.
- Return ProblemDetails for invalid ranges or missing references.
- Preserve the Created response contract.
- Update repository or endpoint tests as needed.

Verification:
- dotnet restore Hourglass/Hourglass.sln
- dotnet build Hourglass/Hourglass.sln
- dotnet test Hourglass/Hourglass.sln
```

### Update User

```text
Task:
Update the user update flow so it preserves the current route contract and validation style.

Constraints:
- Keep the route under Endpoints/Users and preserve authorization.
- Keep the validation filter on the request contract.
- Preserve ProblemDetails for duplicate, invalid, or missing data.
- Update repository behavior and repository tests if persistence rules change.
- Keep OpenAPI metadata complete.

Verification:
- dotnet restore Hourglass/Hourglass.sln
- dotnet build Hourglass/Hourglass.sln
- dotnet test Hourglass/Hourglass.sln
```

### Add New Endpoint

```text
Task:
Add a new Minimal API endpoint following the conventions already used in this repository.

Constraints:
- Register the endpoint inside an extension class under Endpoints/.
- Use strongly typed results and ProblemDetails for failures.
- Add WithSummary, WithDescription, Accepts, and Produces metadata.
- Keep authorization explicit.
- Add tests for the new behavior before merging.

Verification:
- dotnet restore Hourglass/Hourglass.sln
- dotnet build Hourglass/Hourglass.sln
- dotnet test Hourglass/Hourglass.sln
```

### Change Repository Logic

```text
Task:
Change repository behavior for an existing entity without widening the public API.

Constraints:
- Keep input validation explicit.
- Prefer AsNoTracking for read-only queries.
- Preserve logging around create and update operations.
- Add tests for the changed query or state transition.
- Update docs only if the public behavior changes.

Verification:
- dotnet restore Hourglass/Hourglass.sln
- dotnet build Hourglass/Hourglass.sln
- dotnet test Hourglass/Hourglass.sln
```
