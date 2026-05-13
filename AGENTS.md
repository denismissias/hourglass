# Hourglass AI Agent Guide

This repository uses a documentation-first AI workflow: every AI-generated change must be bounded by clear constraints, guided by local documentation, and verified with deterministic checks.

## Project Snapshot

- Stack: ASP.NET Core Minimal API on .NET 10
- Solution: `Hourglass/Hourglass.sln`
- API project: `Hourglass/Hourglass`
- Tests project: `Hourglass/Hourglass.Tests`
- Data access: EF Core + Pomelo MySQL provider
- Auth: JWT Bearer

## Commands

Run from repository root:

```powershell
dotnet restore Hourglass/Hourglass.sln
dotnet build Hourglass/Hourglass.sln
dotnet test Hourglass/Hourglass.sln
```

Or run the standard solution checks directly.

## AI Workflow Contract

When implementing a feature with AI, require these conditions:

1. Keep endpoint behavior explicit and predictable.
2. Keep OpenAPI metadata complete (`WithSummary`, `WithDescription`, `Produces...`).
3. Preserve security boundaries (`RequireAuthorization` where needed).
4. Add or update tests for behavior changes.
5. Pass full solution build + test before merge.

## Minimal API Conventions

1. Register endpoints inside extension classes under `Endpoints/`.
2. Use strongly-typed results (`Results<...>`, `TypedResults`).
3. Return `ProblemDetails` for client and domain errors.
4. Add endpoint validation filters for request contracts.

## Definition Of Done For AI Changes

1. Build succeeds for `Hourglass/Hourglass.sln`.
2. Tests pass for `Hourglass/Hourglass.sln`.
3. New/updated endpoints include OpenAPI metadata.
4. README or docs updated if behavior changed.
