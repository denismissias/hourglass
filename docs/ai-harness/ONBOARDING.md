# AI Onboarding With Documentation First

This guide onboards AI contributors in a controlled way, so generated code is useful and safe for production.

## Why This Workflow Exists

In this project, the workflow wraps AI generation with:

1. Clear constraints (architecture, coding style, contracts).
2. Repeatable checks (build, tests, metadata checks).
3. Fast feedback loop (small change batches + solution checks).

## 20-Minute Onboarding Flow

1. Read `README.md` and `AGENTS.md`.
2. Read `docs/ai-harness/PROJECT-MAP.md` for where each concern lives.
3. Inspect `Hourglass/Hourglass/Program.cs` and one endpoint from each area under `Hourglass/Hourglass/Endpoints/`.
4. Run local baseline validation:

```bash
dotnet restore Hourglass/Hourglass.sln
dotnet build Hourglass/Hourglass.sln
dotnet test Hourglass/Hourglass.sln
```

5. Pick one small task and apply the prompt template below.
6. Re-run the solution checks after code generation.

## Prompt Template For Tasks

Use this template when requesting changes from an AI assistant:

```text
Task:
<what must be implemented>

Constraints:
- Keep Minimal API pattern already used in Endpoints/*.
- Return ProblemDetails for business/validation errors.
- Add OpenAPI metadata: WithSummary, WithDescription, Produces(...).
- Keep authentication and authorization behavior unchanged unless explicitly requested.

Verification:
- dotnet restore Hourglass/Hourglass.sln
- dotnet build Hourglass/Hourglass.sln
- dotnet test Hourglass/Hourglass.sln

Output format:
- List changed files.
- Explain behavior changes.
- Mention any known risks or TODOs.
```

If you want concrete examples, see [docs/ai-harness/PROJECT-MAP.md](PROJECT-MAP.md) for ready-to-use prompts for Authentication, Users, and Times changes.

## Review Harness (PR Checklist)

Use this checklist before merge:

1. Scope control: only files needed for the task were modified.
2. Behavior safety: no accidental changes in auth, versioning, or route contracts.
3. API contract quality: endpoint metadata and response codes still accurate.
4. Test confidence: unit tests cover new logic and edge cases.
5. Operability: no startup regressions in `Program.cs` configuration.

## Anti-Patterns To Avoid

1. Large multi-concern prompts that modify many endpoints at once.
2. Returning anonymous/raw objects instead of typed DTO contracts.
3. Skipping `ProblemDetails` and returning inconsistent error shapes.
4. Merging AI code without running full solution tests.

## Recommended Iteration Size

Keep each AI request bounded to one domain area:

1. Authentication
2. Users
3. Times
4. Repository implementation
5. Service logic

Small iterations reduce hallucination impact and simplify rollback.
