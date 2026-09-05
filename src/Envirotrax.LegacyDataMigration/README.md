# Legacy Data Migration

> ⚠️ **LOCAL DATABASES ONLY.** This app writes data with no undo. Never point it at a shared or production connection string — doing so will corrupt or overwrite real data. Always run it against databases on your own machine. See [Prerequisites](#prerequisites) before running.

Moves data from the old (V1) system into the new Envirotrax2 (V2) database.

## How it works

The app migrates one entity at a time — users, water suppliers, supplier users, then sites — in that order, because later entities depend on earlier ones. Each entity's migration has two steps:

1. **Run SQL scripts.** Files under `Scripts/<Entity>/`, named with a numeric prefix (`01_`, `02_`, ...), run in that order. They copy rows from the legacy database into the V2 tables.
2. **Run C# cleanup code.** After the raw data is in place, C# fixes up anything SQL can't do well — for example, legacy passwords come in as plain text and get hashed with ASP.NET Identity's password hasher.

`Program.cs` wires up the database connections and services, then calls each service's `MigrateAsync()` in order.

## Project layout

- `Scripts/` — SQL scripts, one subfolder per entity, run in filename order.
- `Services/` — one service per entity (`UserService`, `WaterSupplierService`, `WaterSupplierUserService`, `SiteService`). Each has a `MigrateAsync()` that runs its scripts, then does any C#-side cleanup.
- `Data/` — EF Core `DbContext`s and entity models for the V2 database.
- `Logs/` — one log file per service, written while the migration runs.

## Prerequisites

> ⚠️ **Do this before running the app.** Skipping this, or pointing a connection string at the shared V2 database, means you will migrate legacy data on top of everyone else's data.

Both databases below must be **local** — on your own machine, not the shared dev/staging V2 database.

1. **Legacy V1 database.** You should already have the old Vepo database running locally.
2. **Empty V2 database.** This doesn't exist until you create it yourself. Launch the `Envirotrax.Auth` and `Envirotrax.App` projects with their connection strings pointed at your local SQL Server. Running them applies EF Core migrations and creates an empty, correctly-shaped V2 database for you.

Only once both databases exist **locally** should you move on to running this project.

## Running it

Before running, double-check every connection string in `Program.cs` points to `localhost` / `(localdb)` — never to a shared server. Then run the project. Logs print to the console and to `Logs/<ServiceName>.log`.
