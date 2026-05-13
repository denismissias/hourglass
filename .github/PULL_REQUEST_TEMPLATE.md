# Pull Request

## Context

- What problem is this change solving?
- Why is this approach appropriate for Hourglass?

## Change Summary

- [ ] API behavior changed
- [ ] Data access behavior changed
- [ ] Authentication/authorization behavior changed
- [ ] Docs updated

## AI Harness Checklist

- [ ] Scope is bounded (only required files changed)
- [ ] New/updated endpoints include OpenAPI metadata (`WithSummary`, `WithDescription`, `Produces...`)
- [ ] Security boundaries were preserved (`RequireAuthorization` where required)
- [ ] Errors use `ProblemDetails` consistently
- [ ] Behavior changes are covered by tests
- [ ] `dotnet build Hourglass/Hourglass.sln` passed locally
- [ ] `dotnet test Hourglass/Hourglass.sln` passed locally

## Validation Evidence

Paste key results from:

- `dotnet build Hourglass/Hourglass.sln`
- `dotnet test Hourglass/Hourglass.sln`

## Risks And Rollback

- Main risks:
- Rollback strategy:
