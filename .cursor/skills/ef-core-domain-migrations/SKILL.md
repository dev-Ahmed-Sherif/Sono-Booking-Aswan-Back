---
name: ef-core-domain-migrations
description: >-
  End-to-end EF Core migration workflow from domain entity changes: scan Domain
  layer diffs, align DbContext and configuration in Infrastructure, add a
  migration (user-named or derived from changed entities), apply database update,
  and run a dotnet build gate after each substantive step. Use when the user asks
  for migrations after editing entities, syncing DbContext with the domain model,
  dotnet ef migrations add, database update, or keeping the schema aligned with
  SonoTracker.Domain.
---

# EF Core migrations from domain changes

## When this applies

Use this skill for **.NET + EF Core** repos where:

- Entities live under a **Domain** project (e.g. `SonoTracker.Domain`).
- **`DbContext`** and **migrations** live in **Infrastructure** (e.g. `SonoTracker.Infrastructure`).
- A **startup** project (e.g. API) holds connection strings and DI for the context.

Adapt paths and project names to the repo if they differ.

## Hard rules

1. **After every substantive change** (context edits, new migration files, or post-update), run **`dotnet build`** on the **solution** (or the relevant solution if multiple). **Do not continue** while the build fails; fix compile errors first.
2. Prefer **`dotnet ef`** from the **repository root** (or directory containing the solution), with explicit **`--project`** (Infrastructure) and **`--startup-project`** (API/host).
3. Migration **class names** must be valid C# identifiers: PascalCase, no spaces. Replace hyphens/spaces from user input (e.g. `add-inspection-clause` → `AddInspectionClause`).

## Workflow checklist

Copy and track:

```text
- [ ] 1. Locate solution, DbContext, startup project, context class name
- [ ] 2. Scan domain changes (scope + diffs)
- [ ] 3. Update Infrastructure DbContext / model configuration
- [ ] 4. Build → fix until green
- [ ] 5. Choose migration name (user or derived)
- [ ] 6. dotnet ef migrations add → Build → fix until green
- [ ] 7. dotnet ef database update → Build (or full verify) → fix until green
```

### Step 1 — Discover projects

- Find **`.sln`**: may be repo root or parent folder if the workspace is a subfolder.
- Find **`DbContext`**: typically `*DbContext.cs` under `**/Infrastructure/**/Context/`.
- **Startup project**: web API or host that references Infrastructure and registers `DbContext` (e.g. `Program.cs` / `Startup.cs`).

**SonoTracker defaults** (override if the repo changed):

- Context: `SonoTracker.Infrastructure` → `SonoTrackerDbContext`
- Startup: `SonoTracker.Api`
- Solution: `SonoTracker.sln` (often next to or above the backend folder)

### Step 2 — Scan domain / entity changes

- Use **`git status`** / **`git diff`** focused on **`Domain`** (e.g. `SonoTracker.Domain/Entities/**`).
- If the user gave a **narrow scope** (specific entities or feature), limit analysis to that.
- List **added/removed/renamed** entities and **meaningful property/relationship** changes; this informs DbContext updates and migration naming.

### Step 3 — Align `DbContext` (Infrastructure)

For each entity that should be persisted:

- Add or update **`DbSet<T>`** on the context (match existing naming: plural properties, same style as sibling sets).
- Add/update **Fluent API** in `OnModelCreating` (or partials/configuration classes) if the project uses that pattern — **follow existing conventions** in the same file.
- Ensure **relationships**, **keys**, **indexes**, **concurrency**, and **owned types** match the domain intent and existing patterns.

Then go to **Step 4**.

### Step 4 — Build gate (mandatory)

```bash
dotnet build <PathToSolution.sln>
```

Resolve all errors. Warnings are optional unless the team treats them as errors.

### Step 5 — Migration name

- If the user supplied a name: **normalize** to a valid C# identifier (PascalCase, strip invalid chars).
- Else derive a short PascalCase name from the change list, e.g.:
  - New entity `InspectionClause` → `AddInspectionClause` or `AddInspectionClausesTable`
  - Multiple entities → combine sensibly: `AddInspectionClauseAndFloatingUnitClause`
- Avoid generic names like `UpdateDatabase` unless nothing more specific fits.

### Step 6 — Add migration

```bash
dotnet ef migrations add <MigrationName> \
  --project <Infrastructure.csproj> \
  --startup-project <Startup.csproj> \
  --context <DbContextClassName>
```

- Review generated **migration `Up`/`Down`** for accidental drops or data loss; adjust if the project expects safe evolution (defaults, renames, etc.).
- Run **Step 4** again (`dotnet build`).

### Step 7 — Update database

```bash
dotnet ef database update \
  --project <Infrastructure.csproj> \
  --startup-project <Startup.csproj> \
  --context <DbContextClassName>
```

- Requires a **valid connection string** for the target environment (often local from `appsettings.Development.json`). If update would hit the wrong database, **stop** and confirm with the user.
- Run **Step 4** again (final build gate).

## EF tools

If `dotnet ef` is not found:

```bash
dotnet tool install --global dotnet-ef
```

Use a version compatible with the repo’s EF Core packages.

## Naming and safety notes

- **Never** run `database update` against production without explicit user confirmation.
- If the migration is **empty** (no model changes), consider removing it and fixing the model/DbContext first — avoid noise migrations unless intentional.
- Keep the skill **DRY**: do not duplicate large command blocks; re-run the same build command after each phase.

## Quick reference (SonoTracker-shaped repos)

```bash
dotnet ef migrations add <MigrationName> \
  --project SonoTracker.Infrastructure/SonoTracker.Infrastructure.csproj \
  --startup-project SonoTracker.Api/SonoTracker.Api.csproj \
  --context SonoTrackerDbContext

dotnet ef database update \
  --project SonoTracker.Infrastructure/SonoTracker.Infrastructure.csproj \
  --startup-project SonoTracker.Api/SonoTracker.Api.csproj \
  --context SonoTrackerDbContext
```

Adjust paths if the solution file lives in a parent directory (use correct relative paths from where you run the command).
