# Legacy Data Migration

> ⚠️ **LOCAL DATABASES ONLY.** This app writes data with no undo. Never point it at a shared or production connection string — doing so will corrupt or overwrite real data. Always run it against databases on your own machine. See [Prerequisites](#prerequisites) before running.

Moves data from the old (V1) system into the new Envirotrax2 (V2) database.

## How it works

The app migrates one entity at a time — users, water suppliers, supplier users, sites, then site logs — in that order, because later entities depend on earlier ones. Each entity's migration has two steps:

1. **Run SQL scripts.** Files under `Scripts/<Entity>/`, named with a numeric prefix (`01_`, `02_`, ...), run in that order. They copy rows from the legacy database into the V2 tables.
2. **Run C# cleanup code.** After the raw data is in place, C# fixes up anything SQL can't do well — legacy passwords come in as plain text and get hashed with ASP.NET Identity's password hasher, and site log file attachments get downloaded from the legacy file server and uploaded to Azure Storage.

`Program.cs` wires up the database connections, the file server address, the storage account and the services, then calls each service's `MigrateAsync()` in order.

**Re-running is safe.** Every insert into a real table is guarded, so running the app again creates no duplicate records and re-does no upload that already succeeded. That is also how you retry: if some attachments failed because the network was down, just run the whole app again — it walks past everything that is already migrated and picks up only what is still pending.

One caveat, on the audit tables rather than the data: `Scripts/Users/02_`–`06_` re-insert into `MigrationSkippedUsers` on every run, and their "UserID already exists in AspNetUsers" reason matches every user the previous run migrated successfully. So after a second run that table describes the state it found, not users it rejected. `MigrationSkippedSites` and `MigrationSkippedSiteLogs` are guarded and stay accurate.

## Project layout

- `Scripts/` — SQL scripts, one subfolder per entity, run in filename order.
- `Services/` — one service per entity (`UserService`, `WaterSupplierService`, `WaterSupplierUserService`, `SiteService`, `SiteLogService`). Each has a `MigrateAsync()` that runs its scripts, then does any C#-side cleanup. `LegacyFileServerService` and `BlobStorageService` are helpers rather than entity migrations: they read files off the legacy file server and write them to Azure Storage.
- `Data/` — EF Core `DbContext`s and entity models for the V2 database.
- `Logs/` — one log file per service, written while the migration runs.

## Prerequisites

> ⚠️ **Do this before running the app.** Skipping this, or pointing a connection string at the shared V2 database, means you will migrate legacy data on top of everyone else's data.

Both databases below must be **local** — on your own machine, not the shared dev/staging V2 database.

1. **Legacy V1 database.** You should already have the old Vepo database running locally.
2. **Empty V2 database.** This doesn't exist until you create it yourself. Launch the `Envirotrax.Auth` and `Envirotrax.App` projects with their connection strings pointed at your local SQL Server. Running them applies EF Core migrations and creates an empty, correctly-shaped V2 database for you.

Only once both databases exist **locally** should you move on to running this project.

3. **Azure sign-in, plus access to the legacy file server.** The site log migration downloads every V1 file attachment from `https://iofiles.envirotrax.com` and uploads it to Azure Blob Storage, so the machine needs outbound access to that server and you need to be signed in to Azure (`az login`, or a signed-in Visual Studio) as an account holding the **Storage Blob Data Contributor** role on the storage account named in `Program.cs`. The app checks this with a probe upload before it starts; if the check fails it logs an error, moves no files, and leaves every attachment pending for a later run.

> ⚠️ **The storage account is shared, and there is no local substitute for it.** Unlike the two databases, blob storage has no `(localdb)` equivalent, so a local run still writes into the real `envirotrax2dev` storage account. Re-running against the **same** V2 database is bounded: the blob name is fixed by the site's row id plus the V1 record id, so a retry overwrites the same file instead of piling up a new one. Rebuilding the V2 database from scratch is **not** bounded — `Sites.Id` is reassigned, so the next run writes the same files under different folders and leaves the previous run's blobs behind. Know what you are pointing at before you run.

## How site log attachments are migrated

V1 kept no path for an attachment; it rebuilt the location from the record id every time it rendered a link. The SQL script therefore writes that rebuilt location into `SiteLogs.LegacyFilePath` (for example `site_log/17390000/17394521.pdf`), leaves `FileAttachmentPath` NULL, and sets `SkipFile`. `SiteLogService` then downloads each file, uploads it to Azure, and in one save writes `FileAttachmentPath` and clears `SkipFile`.

This ordering is what keeps the UI honest: while an attachment is still pending or has permanently failed, V2 shows its file name as plain text rather than a link that goes nowhere; it becomes a link only once the file is provably in Azure.

Two queries tell you where things stand:

- **Still pending:** `SELECT COUNT(*) FROM SiteLogs WHERE LegacyFilePath IS NOT NULL AND FileAttachmentPath IS NULL`
- **Permanently unmigratable:** `SELECT * FROM MigrationSkippedSiteLogs WHERE SourceTable = 'CsiBackflowSiteLog.FileAttachment'` — V1 rows whose file type is missing or unusable, so there is no legacy file name to fetch. Everything else is logged to `Logs/SiteLogService.log` with per-run counts.

## Running it

Before running, double-check every connection string in `Program.cs` points to `localhost` / `(localdb)` — never to a shared server — and check the storage account name right below them. Then run the project. Logs print to the console and to `Logs/<ServiceName>.log`.
